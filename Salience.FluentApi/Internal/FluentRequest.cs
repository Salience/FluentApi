using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Salience.FluentApi.Internal
{
    internal class FluentRequest : IEmptyRequest, IRequestWithOperation, IRequestWithMethod, IRequestWithExpectedStatus
    {
        protected readonly FluentClient _client;
        protected readonly RequestData _data;

        public FluentRequest(FluentClient client, RequestData data)
        {
            _client = client;
            _data = data;
        }

        IRequestWithOperation IEmptyRequest.To(string operation)
        {
            Guard.NotNullOrEmpty(operation, "operation");

            _data.Operation = operation;
            return this;
        }

        private IRequestWithMethod SetMethodAndPath(Method method, string resourcePath, Action<RestRequest> requestCustomizer = null)
        {
            Guard.NotNullOrEmpty(resourcePath, "resourcePath");

            _data.Method = method;
            _data.ResourcePath = "/" + resourcePath.Trim('/');
            _data.RequestCustomizer = requestCustomizer;
            return this;
        }

        IRequestWithMethod IRequestWithOperation.Get(string resourcePath)
        {
            return this.SetMethodAndPath(Method.GET, resourcePath);
        }

        IRequestWithMethod IRequestWithOperation.Get(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.SetMethodAndPath(Method.GET, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Post(string resourcePath)
        {
            return this.SetMethodAndPath(Method.POST, resourcePath);
        }

        IRequestWithMethod IRequestWithOperation.Post(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.SetMethodAndPath(Method.POST, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Put(string resourcePath)
        {
            return this.SetMethodAndPath(Method.PUT, resourcePath);
        }

        IRequestWithMethod IRequestWithOperation.Put(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.SetMethodAndPath(Method.PUT, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Patch(string resourcePath)
        {
            return this.SetMethodAndPath(Method.PATCH, resourcePath);
        }

        IRequestWithMethod IRequestWithOperation.Patch(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.SetMethodAndPath(Method.PATCH, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Delete(string resourcePath)
        {
            return this.SetMethodAndPath(Method.DELETE, resourcePath);
        }

        IRequestWithMethod IRequestWithOperation.Delete(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.SetMethodAndPath(Method.DELETE, resourcePath, requestCustomizer);
        }

        IRequestWithUrl IRequestWithMethod.UsingBase(string otherBaseApiPath)
        {
            Guard.NotNullOrEmpty(otherBaseApiPath, "otherBaseApiPath");

            otherBaseApiPath = otherBaseApiPath.Trim();
            _data.BaseApiPath = string.IsNullOrEmpty(otherBaseApiPath) ? "" : "/" + otherBaseApiPath.Trim('/');
            return this;
        }

        IRequestWithExpectedStatus IRequestWithUrl.Expecting(HttpStatusCode expectedStatusCode, params HttpStatusCode[] otherAcceptedStatusCodes)
        {
            _data.ExpectedStatusCodes.Add(expectedStatusCode);
            foreach (var code in otherAcceptedStatusCodes)
                _data.ExpectedStatusCodes.Add(code);
            return this;
        }

        IRequestWithContent<T> IRequestWithUrl.Expecting<T>()
        {
            _data.ResponseBodyType = typeof(T);
            return new FluentRequestWithContent<T>(_client, _data);
        }

        IRequestWithContent<TResult> IRequestWithUrl.Expecting<TResponse, TResult>(Func<TResponse, TResult> resultGetter)
        {
            Guard.NotNull(resultGetter, "resultGetter");

            _data.ResponseBodyType = typeof(TResponse);
            _data.ResultGetter = resultGetter;
            return new FluentRequestWithContent<TResult>(_client, _data);
        }

        IRequestWithContent<string> IRequestWithExpectedStatus.WithRawContent()
        {
            _data.UseRawResponseContent = true;
            return new FluentRequestWithContent<string>(_client, _data);
        }

        IRequestWithContent<TResult> IRequestWithExpectedStatus.WithRawContent<TResult>(Func<string, TResult> resultGetter)
        {
            _data.UseRawResponseContent = true;
            _data.ResponseBodyType = typeof(TResult);
            _data.ResultGetter = resultGetter;
            return new FluentRequestWithContent<TResult>(_client, _data);
        }

        IRequestWithContent<T> IRequestWithExpectedStatus.WithContent<T>()
        {
            return ((IRequestWithUrl)this).Expecting<T>();
        }

        IRequestWithContent<TResult> IRequestWithExpectedStatus.WithContent<TResponse, TResult>(Func<TResponse, TResult> resultGetter)
        {
            return ((IRequestWithUrl)this).Expecting(resultGetter);
        }

        void IExecutableRequest.Execute()
        {
            _client.HandleRequest(_data);
        }

        Task IExecutableRequest.ExecuteAsync(CancellationToken token)
        {
            return _client.HandleRequestAsync(_data, token);
        }


        IExecutableRequest IExecutableRequest.AndThen(IExecutableRequest otherRequest)
        {
            _data.FollowUps.Add(_ => new VoidExecutableRequestWrapper(otherRequest));
            return this;
        }

        IExecutableRequest<T> IExecutableRequest.AndThen<T>(IExecutableRequest<T> otherRequest)
        {
            _data.FollowUps.Add(_ => new ExecutableRequestWithContentWrapper<T>(otherRequest));
            return new FluentRequestWithContent<T>(_client, _data);
        }
    }

    internal class FluentRequestWithContent<T> : FluentRequest, IRequestWithContent<T>
    {
        public FluentRequestWithContent(FluentClient client, RequestData data)
            : base(client, data)
        {
        }

        IRequestWithAlternateResult<T> IRequestWithContent<T>.Or(T alternateResult)
        {
            return new FluentRequestWithAlternateContent<T>(this, _data, alternateResult);
        }

        IRequestWithContent<T> IRequestWithContent<T>.OrDefaultIfNotFound()
        {
            return ((IRequestWithContent<T>)this).Or(default(T)).IfNotFound();
        }

        T IExecutableRequest<T>.Execute()
        {
            return (T) _client.HandleRequest(_data);
        }

        async Task<T> IExecutableRequest<T>.ExecuteAsync(CancellationToken token)
        {
            return (T) await _client.HandleRequestAsync(_data, token);
        }

        IExecutableRequest IExecutableRequest<T>.AndThen(Func<T, IExecutableRequest> otherRequest)
        {
            _data.FollowUps.Add(result => new VoidExecutableRequestWrapper(otherRequest((T)result)));
            return new FluentRequest(_client, _data);
        }

        IExecutableRequest<T2> IExecutableRequest<T>.AndThen<T2>(Func<T, IExecutableRequest<T2>> otherRequest)
        {
            _data.FollowUps.Add(result => new ExecutableRequestWithContentWrapper<T2>(otherRequest((T)result)));
            return new FluentRequestWithContent<T2>(_client, _data);
        }
    }

    internal class FluentRequestWithAlternateContent<T> : IRequestWithAlternateResult<T>
    {
        private readonly FluentRequestWithContent<T> _request;
        private readonly RequestData _data;
        private readonly object _alternateContent;

        public FluentRequestWithAlternateContent(FluentRequestWithContent<T> request, RequestData description, object alternateContent)
        {
            _request = request;
            _data = description;
            _alternateContent = alternateContent;
        }

        IRequestWithContent<T> IRequestWithAlternateResult<T>.If(HttpStatusCode expectedStatusCode, params HttpStatusCode[] otherAcceptedStatusCodes)
        {
            _data.ExpectedStatusCodes.Add(expectedStatusCode);
            _data.AlternateResults.Add(expectedStatusCode, _alternateContent);

            foreach(var code in otherAcceptedStatusCodes)
            {
                _data.ExpectedStatusCodes.Add(code);
                _data.AlternateResults.Add(code, _alternateContent);
            }

            return _request;
        }

        IRequestWithContent<T> IRequestWithAlternateResult<T>.IfNotFound()
        {
            return ((IRequestWithAlternateResult<T>)this).If(HttpStatusCode.NotFound);
        }

        IRequestWithContent<T> IRequestWithAlternateResult<T>.IfNoContent()
        {
            return ((IRequestWithAlternateResult<T>)this).If(HttpStatusCode.NoContent);
        }
    }
}