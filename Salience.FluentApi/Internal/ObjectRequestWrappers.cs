using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi.Internal
{
    internal class VoidExecutableRequestWrapper : IExecutableRequest<object>
    {
        private readonly IExecutableRequest _request;

        public VoidExecutableRequestWrapper(IExecutableRequest request)
        {
            _request = request;
        }

        public IExecutableRequest AndThen(Func<object, IExecutableRequest> otherRequest)
            => _request.AndThen(otherRequest(null));

        public IExecutableRequest<T2> AndThen<T2>(Func<object, IExecutableRequest<T2>> otherRequest)
            => _request.AndThen(otherRequest(null));

        public object Execute()
        {
            _request.Execute();
            return null;
        }

        public async Task<object> ExecuteAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            await _request.ExecuteAsync(token);
            return null;
        }
    }

    internal class ExecutableRequestWithContentWrapper<T> : IExecutableRequest<object>
    {
        private readonly IExecutableRequest<T> _request;

        public ExecutableRequestWithContentWrapper(IExecutableRequest<T> request)
        {
            _request = request;
        }

        public IExecutableRequest AndThen(Func<object, IExecutableRequest> otherRequest)
            => _request.AndThen(result => otherRequest(result));

        public IExecutableRequest<T2> AndThen<T2>(Func<object, IExecutableRequest<T2>> otherRequest)
            => _request.AndThen(result => otherRequest(result));

        public object Execute()
            => _request.Execute();

        public async Task<object> ExecuteAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return await _request.ExecuteAsync(token);
        }
    }
}
