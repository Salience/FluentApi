using Salience.FluentApi.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi.Internal
{
    internal class FinalActionWrapper : IFinalExecutableRequest<object>
    {
        private readonly Action _action;

        public FinalActionWrapper(Action action)
        {
            _action = action;
        }

        public object Execute()
        { 
            _action();
            return null;
        }
        
        public Task<object> ExecuteAsync(CancellationToken token = default)
        {
            _action();
            return Task.FromResult<object>(null);
        }
    }

    internal class FinalFuncWrapper<T> : IFinalExecutableRequest<object>
    {
        private readonly Func<T> _func;

        public FinalFuncWrapper(Func<T> func)
        {
            _func = func;
        }

        public object Execute() => _func();
        public Task<object> ExecuteAsync(CancellationToken token = default) => Task.FromResult<object>(_func());
    }
}
