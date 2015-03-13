using System.Net;

namespace Salience.FluentApi
{
    public interface IRequestWithUrl : IExecutableRequest
    {
        /// <summary>
        /// Specifies the status code to expect as response.
        /// </summary>
        /// <param name="expectedStatusCode">The expected response status code</param>
        /// <param name="otherAcceptedStatusCodes">Other accepted status code as response</param>
        /// <returns>This request</returns>
        IRequestWithExpectedStatus Expecting(HttpStatusCode expectedStatusCode, params HttpStatusCode[] otherAcceptedStatusCodes);

        /// <summary>
        /// Specifies the type of the expected response body.
        /// </summary>
        /// <typeparam name="T">The type of the expected response body</typeparam>
        /// <returns>This request</returns>
        IExecutableRequest<T> Expecting<T>();
    }
}