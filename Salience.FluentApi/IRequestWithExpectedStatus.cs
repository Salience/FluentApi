using System;

namespace Salience.FluentApi
{
    public interface IRequestWithExpectedStatus : IExecutableRequest
    {
        /// <summary>
        /// Specifies that the request result should be the raw string content received.
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<string> WithRawContent();

        /// <summary>
        /// Specifies that the request result should be produced from the raw string content received.
        /// </summary>
        /// <returns>This request</returns>
        IRequestWithContent<TResult> WithRawContent<TResult>(Func<string, TResult> resultGetter);

        /// <summary>
        /// Specifies the type of the expected response body.
        /// </summary>
        /// <typeparam name="T">The type of the expected response body</typeparam>
        /// <returns>This request</returns>
        IRequestWithContent<T> WithContent<T>();

        /// <summary>
        /// Specifies how the request result should be inferred from the response body.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected response body</typeparam>
        /// <typeparam name="TResult">The type of the result to get from the response</typeparam>
        /// <param name="resultGetter">How to infer the request result from the response body</param>
        /// <returns>This request</returns>
        IRequestWithContent<TResult> WithContent<TResponse, TResult>(Func<TResponse, TResult> resultGetter);
    }
}