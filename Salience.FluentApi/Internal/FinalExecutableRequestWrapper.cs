using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi.Internal
{
    internal class FinalExecutableRequestWrapper : IFinalExecutableRequest<object>
    {
        private readonly IFinalExecutableRequest _request;

        public FinalExecutableRequestWrapper(IFinalExecutableRequest request)
        {
            _request = request;
        }

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

    internal class FinalExecutableRequestWrapper<T> : IFinalExecutableRequest<object>
    {
        private readonly IFinalExecutableRequest<T> _request;

        public FinalExecutableRequestWrapper(IFinalExecutableRequest<T> request)
        {
            _request = request;
        }

        public object Execute() => _request.Execute();

        public async Task<object> ExecuteAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return await _request.ExecuteAsync(token);
        }
    }
}
