using Salience.FluentApi.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi
{
    /// <summary>
    /// Static class providing implementations of <see cref="IExecutableRequest">IExecutableRequest</see>
    /// for results that are already known.
    /// </summary>
    public static class CompletedRequest
    {
        /// <summary>
        /// Gets an <see cref="IExecutableRequest">IExecutableRequest</see> that does not do or return anything.
        /// </summary>
        public static IExecutableRequest Empty => new CompletedRequestWrapper<object>(null);

        /// <summary>
        /// Gets an <see cref="IExecutableRequest">IExecutableRequest</see> that doesn't do anything
        /// and just returns the given result.
        /// </summary>
        /// <typeparam name="T">The type of the returned result.</typeparam>
        /// <param name="result">The result returned by this request.</param>
        /// <returns>The <see cref="IExecutableRequest">IExecutableRequest</see>.</returns>
        public static IExecutableRequest<T> FromResult<T>(T result) => Empty.FollowedBy(() => result);

        private class CompletedRequestWrapper<T> : IExecutableRequest, IExecutableRequest<T>
        {
            private List<FollowUpRequestProvider> _steps;

            public CompletedRequestWrapper(List<FollowUpRequestProvider> previousSteps = null)
            {
                _steps = previousSteps ?? new List<FollowUpRequestProvider>();
            }

            IExecutableRequest IExecutableRequest.FollowedByRequest(IExecutableRequest otherRequest)
            {
                _steps.Add(_ => new FinalExecutableRequestWrapper(otherRequest));
                return this;
            }

            IExecutableRequest IExecutableRequest.FollowedBy(Action action)
            {
                _steps.Add(_ => new FinalActionWrapper(action));
                return this;
            }

            IExecutableRequest IExecutableRequest<T>.FollowedByRequest(Func<T, IExecutableRequest> otherRequest)
            {
                _steps.Add(result => new FinalExecutableRequestWrapper(otherRequest((T)result)));
                return this;
            }

            IExecutableRequest IExecutableRequest<T>.FollowedBy(Action<T> operation)
            {
                _steps.Add(result => new FinalActionWrapper(() => operation((T)result)));
                return this;
            }

            IExecutableRequest<T2> IExecutableRequest.FollowedByRequest<T2>(IExecutableRequest<T2> otherRequest)
            {
                _steps.Add(_ => new FinalExecutableRequestWrapper<T2>(otherRequest));
                return new CompletedRequestWrapper<T2>(_steps);
            }

            IExecutableRequest<T2> IExecutableRequest.FollowedBy<T2>(Func<T2> operation)
            {
                _steps.Add(_ => new FinalFuncWrapper<T2>(operation));
                return new CompletedRequestWrapper<T2>(_steps);
            }

            IExecutableRequest<T2> IExecutableRequest<T>.FollowedByRequest<T2>(Func<T, IExecutableRequest<T2>> otherRequest)
            {
                _steps.Add(result => new FinalExecutableRequestWrapper<T2>(otherRequest((T)result)));
                return new CompletedRequestWrapper<T2>(_steps);
            }

            IExecutableRequest<T2> IExecutableRequest<T>.FollowedBy<T2>(Func<T, T2> transformation)
            {
                _steps.Add(result => new FinalFuncWrapper<T2>(() => transformation((T)result)));
                return new CompletedRequestWrapper<T2>(_steps);
            }

            private object Execute()
            {
                object result = null;
                foreach (var step in _steps)
                    result = step(result).Execute();
                return result;
            }

            private async Task<object> ExecuteAsync(CancellationToken token)
            {
                token.ThrowIfCancellationRequested();

                object result = null;
                foreach (var step in _steps)
                    result = await step(result).ExecuteAsync(token);
                return result;
            }

            T IFinalExecutableRequest<T>.Execute() => (T)this.Execute();

            async Task<T> IFinalExecutableRequest<T>.ExecuteAsync(CancellationToken token) => (T)await this.ExecuteAsync(token);

            void IFinalExecutableRequest.Execute() => this.Execute();

            async Task IFinalExecutableRequest.ExecuteAsync(CancellationToken token) => await this.ExecuteAsync(token);
        }
    }
}
