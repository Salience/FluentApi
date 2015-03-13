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
        Task ExecuteAsync();
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
        Task<T> ExecuteAsync();
    }
}