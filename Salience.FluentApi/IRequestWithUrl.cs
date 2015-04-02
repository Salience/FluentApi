using System;
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
        IRequestWithContent<T> Expecting<T>();

        /// <summary>
        /// Specifies how the request result should be inferred from the response body.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected response body</typeparam>
        /// <typeparam name="TResult">The type of the result to get from the response</typeparam>
        /// <param name="resultGetter">How to infer the request result from the response body</param>
        /// <returns>This request</returns>
        IRequestWithContent<TResult> Expecting<TResponse, TResult>(Func<TResponse, TResult> resultGetter);
    }
}