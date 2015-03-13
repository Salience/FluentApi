using System;
using RestSharp;

namespace Salience.FluentApi
{
    public interface IRequestWithOperation
    {
        /// <summary>
        /// Indicates that the specified resource shall be accessed with the GET method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <returns>This request</returns>
        IRequestWithMethod Get(string resourcePath);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the GET method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <param name="requestCustomizer">A method to customize the RestSharp request</param>
        /// <returns>This request</returns>
        IRequestWithMethod Get(string resourcePath, Action<RestRequest> requestCustomizer);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the POST method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <returns>This request</returns>
        IRequestWithMethod Post(string resourcePath);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the POST method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <param name="requestCustomizer">A method to customize the RestSharp request</param>
        /// <returns>This request</returns>
        IRequestWithMethod Post(string resourcePath, Action<RestRequest> requestCustomizer);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the PUT method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <returns>This request</returns>
        IRequestWithMethod Put(string resourcePath);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the PUT method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <param name="requestCustomizer">A method to customize the RestSharp request</param>
        /// <returns>This request</returns>
        IRequestWithMethod Put(string resourcePath, Action<RestRequest> requestCustomizer);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the DELETE method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <returns>This request</returns>
        IRequestWithMethod Delete(string resourcePath);

        /// <summary>
        /// Indicates that the specified resource shall be accessed with the DELETE method.
        /// </summary>
        /// <param name="resourcePath">The resource to access</param>
        /// <param name="requestCustomizer">A method to customize the RestSharp request</param>
        /// <returns>This request</returns>
        IRequestWithMethod Delete(string resourcePath, Action<RestRequest> requestCustomizer);
    }
}