using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi
{
    public static class CompletedRequest
    {
        public static readonly IExecutableRequest Empty = new CompletedRequestWrapper();

        public static IExecutableRequest<T> FromResult<T>(T result) => new CompletedRequestWrapper<T>(result);

        private class CompletedRequestWrapper : IExecutableRequest
        {
            public IExecutableRequest AndThen(IExecutableRequest otherRequest) => otherRequest;
            public IExecutableRequest<T> AndThen<T>(IExecutableRequest<T> otherRequest) => otherRequest;

            public void Execute() { }
            public Task ExecuteAsync(CancellationToken token = default) => Task.CompletedTask;
        }

        private class CompletedRequestWrapper<T> : IExecutableRequest<T>
        {
            private readonly T _result;

            public CompletedRequestWrapper(T result)
            {
                _result = result;
            }

            public IExecutableRequest AndThen(Func<T, IExecutableRequest> otherRequest) => otherRequest(_result);
            public IExecutableRequest<T2> AndThen<T2>(Func<T, IExecutableRequest<T2>> otherRequest) => otherRequest(_result);

            public T Execute() => _result;
            public Task<T> ExecuteAsync(CancellationToken token = default) => Task.FromResult(_result);
        }
    }
}
