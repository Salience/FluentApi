using System;
using System.Net;

namespace Salience.FluentApi
{
    public interface IRequestWithContent<T> : IExecutableRequest<T>
    {
        /// <summary>
        /// Specifies a default result for the request when receiving a 404 - NotFound response
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<T> OrDefaultIfNotFound();

        /// <summary>
        /// Specifies an alternate result for the request
        /// </summary>
        /// <param name="alternateResult">The alternate result</param>
        /// <returns>This request</returns>
        IRequestWithAlternateResult<T> Or(T alternateResult);
    }

    public interface IRequestWithAlternateResult<T>
    {
        /// <summary>
        /// Return the alternate result when receiving a 404 - NotFound response
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<T> If(HttpStatusCode expectedStatusCode, params HttpStatusCode[] otherAcceptedStatusCodes);

        /// <summary>
        /// Return the alternate result when receiving a 404 - NotFound response
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<T> IfNotFound();

        /// <summary>
        /// Return the alternate result when receiving a 204 - NoContent response
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<T> IfNoContent();
    }
}