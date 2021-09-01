using System;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi
{
    public interface IExecutableRequest
    {
        /// <summary>
        /// Executes the request synchronously.
        /// </summary>
        void Execute();

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        Task ExecuteAsync(CancellationToken token = default);

        IExecutableRequest AndThen(IExecutableRequest otherRequest);

        IExecutableRequest<T> AndThen<T>(IExecutableRequest<T> otherRequest);
    }

    public interface IExecutableRequest<T>
    {
        /// <summary>
        /// Executes the request synchronously.
        /// </summary>
        /// <returns>The deserialized response body</returns>
        T Execute();

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        /// <returns>The deserialized response body</returns>
        Task<T> ExecuteAsync(CancellationToken token = default);

        IExecutableRequest AndThen(Func<T, IExecutableRequest> otherRequest);

        IExecutableRequest<T2> AndThen<T2>(Func<T, IExecutableRequest<T2>> otherRequest);
    }
}