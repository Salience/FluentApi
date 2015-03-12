using System;
using RestSharp;

namespace Salience.FluentApi
{
    public interface IRequestWithOperation
    {
        IRequestWithMethod Get(string resourcePath);
        IRequestWithMethod Get(string resourcePath, Action<RestRequest> requestCustomizer);

        IRequestWithMethod Post(string resourcePath);
        IRequestWithMethod Post(string resourcePath, Action<RestRequest> requestCustomizer);

        IRequestWithMethod Put(string resourcePath);
        IRequestWithMethod Put(string resourcePath, Action<RestRequest> requestCustomizer);

        IRequestWithMethod Delete(string resourcePath);
        IRequestWithMethod Delete(string resourcePath, Action<RestRequest> requestCustomizer);
    }
}