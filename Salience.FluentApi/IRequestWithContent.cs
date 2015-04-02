using System;
using System.Net;

namespace Salience.FluentApi
{
    public interface IRequestWithContent<T> : IExecutableRequest<T>
    {
        /// <summary>
        /// Specifies a default result for the request when receiving a 404 - NotFound response
        /// </summary>
        /// <param name="defaultResult">The default result</param>
        /// <returns>This request</returns>
        IExecutableRequest<T> OrIfNotFound(T defaultResult);

        /// <summary>
        /// Specifies that a default result will be returned for the request when receiving a 404 - NotFound response
        /// </summary>
        /// <returns>This request</returns>
        IExecutableRequest<T> OrDefaultIfNotFound();
    }
}