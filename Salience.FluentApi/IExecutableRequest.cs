using System;
using System.Threading;
using System.Threading.Tasks;

namespace Salience.FluentApi
{
    public interface IFinalExecutableRequest
    {
        /// <summary>
        /// Executes the request synchronously.
        /// </summary>
        void Execute();

        /// <summary>
        /// Executes the request asynchronously.
        /// </summary>
        Task ExecuteAsync(CancellationToken token = default);
    }

    public interface IExecutableRequest : IFinalExecutableRequest
    {
        /// <summary>
        /// Indicates another request to be executed immediately after this request.
        /// </summary>
        /// <param name="otherRequest">The other request to execute afterward.</param>
        IExecutableRequest FollowedByRequest(IExecutableRequest otherRequest);

        /// <summary>
        /// Indicates an action to execute immediately after this request.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        IExecutableRequest FollowedBy(Action action);

        /// <summary>
        /// Indicates another request to be executed immediately after this request.
        /// </summary>
        /// <param name="otherRequest">The other request to execute afterward.</param>
        IExecutableRequest<T> FollowedByRequest<T>(IExecutableRequest<T> otherRequest);

        /// <summary>
        /// Indicates an operation to execute immediately after this request.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        IExecutableRequest<T> FollowedBy<T>(Func<T> operation);
    }

    public interface IFinalExecutableRequest<T>
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
    }

    public interface IExecutableRequest<T> : IFinalExecutableRequest<T>
    {
        /// <summary>
        /// Indicates another request to be executed immediately after this request.
        /// </summary>
        /// <param name="otherRequest">The other request to execute afterward.</param>
        IExecutableRequest FollowedByRequest(Func<T, IExecutableRequest> otherRequest);

        /// <summary>
        /// Indicates an action to execute immediately after this request based on its result.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        IExecutableRequest FollowedBy(Action<T> operation);

        /// <summary>
        /// Indicates another request to be executed immediately after this request.
        /// </summary>
        /// <param name="otherRequest">The other request to execute afterward.</param>
        IExecutableRequest<T2> FollowedByRequest<T2>(Func<T, IExecutableRequest<T2>> otherRequest);

        /// <summary>
        /// Indicates an operation to transform the result of this request into a different result.
        /// </summary>
        /// <param name="transformation">The operation to transform the original result.</param>
        IExecutableRequest<T2> FollowedBy<T2>(Func<T, T2> transformation);
    }
}