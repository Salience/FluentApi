using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Salience.FluentApi
{
    internal class FluentRequest : IRequestWithOperation, IRequestWithMethod, IRequestWithExpectedStatus
    {
        protected readonly RequestDescription _desc;

        public FluentRequest(RequestDescription description)
        {
            _desc = description;
        }

        private void TraceRequest()
        {
            var requestUri = _desc.RestClient.BuildUri(_desc.Request);
            var requestBodyParameter = _desc.Request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            var requestBodyContent = requestBodyParameter == null ? "(no body)" : requestBodyParameter.Value;
            _desc.Trace(TraceLevel.Debug, "Trying to {0} by sending {1} request to {2} : {3}", _desc.Operation, _desc.Request.Method, requestUri, requestBodyContent);
        }

        private void ValidateResponse()
        {
            var response = _desc.Response;

            if(response.ErrorException != null)
            {
                _desc.TraceError(TraceLevel.Error, response.ErrorException, "Could not {0} (transport level error): {1}", _desc.Operation, response.ErrorMessage);
                throw new RestException("Transport level error: " + response.ErrorMessage, response.ErrorException);
            }

            string responseContent = response.Content ?? "(no content)";

            if(!_desc.ExpectedStatusCodes.Contains(response.StatusCode))
            {
                _desc.TraceError(TraceLevel.Error, response.ErrorException, "Could not {0} (wrong status returned - {1}): {2}", _desc.Operation, response.StatusCode, responseContent);
                throw new RestException("Wrong status returned: " + response.StatusDescription, response.Content, response.StatusCode);
            }

            _desc.Trace(TraceLevel.Debug, "Received response to {0}: {1}", _desc.Operation, responseContent);
        }

        private IRequestWithMethod DescribeRequest(Method method, string resourcePath, Action<RestRequest> requestCustomizer)
        {
            Guard.NotNullOrEmpty(resourcePath, "resourcePath");
            
            try
            {
                _desc.ResourcePath = "/" + resourcePath.Trim('/');
                _desc.Request = new RestRequest
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new JsonDotNetSerializerWrapper(_desc.Serializer),
                    Method = method
                };

                if(requestCustomizer != null)
                    requestCustomizer(_desc.Request);

                if((method == Method.POST || method == Method.PUT)
                   && !_desc.Request.Parameters.Any(p => p.Type == ParameterType.HttpHeader && p.Name == "Content-Type"))
                    _desc.Request.AddHeader("Content-Type", "application/json");

                return this;
            }
            catch(Exception ex)
            {
                _desc.TraceError(TraceLevel.Error, ex, "Could not {0} (error while creating request): {1}", _desc.Operation, ex.Message);
                throw new RestException("Error while creating request: " + ex.Message, ex);
            }
        }

        IRequestWithMethod IRequestWithOperation.Get(string resourcePath)
        {
            return this.DescribeRequest(Method.GET, resourcePath, null);
        }

        IRequestWithMethod IRequestWithOperation.Get(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.DescribeRequest(Method.GET, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Post(string resourcePath)
        {
            return this.DescribeRequest(Method.POST, resourcePath, null);
        }

        IRequestWithMethod IRequestWithOperation.Post(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.DescribeRequest(Method.POST, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Put(string resourcePath)
        {
            return this.DescribeRequest(Method.PUT, resourcePath, null);
        }

        IRequestWithMethod IRequestWithOperation.Put(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.DescribeRequest(Method.PUT, resourcePath, requestCustomizer);
        }

        IRequestWithMethod IRequestWithOperation.Delete(string resourcePath)
        {
            return this.DescribeRequest(Method.DELETE, resourcePath, null);
        }

        IRequestWithMethod IRequestWithOperation.Delete(string resourcePath, Action<RestRequest> requestCustomizer)
        {
            return this.DescribeRequest(Method.DELETE, resourcePath, requestCustomizer);
        }

        IRequestWithUrl IRequestWithMethod.UsingBase(string otherBaseApiPath)
        {
            Guard.NotNullOrEmpty(otherBaseApiPath, "otherBaseApiPath");

            otherBaseApiPath = otherBaseApiPath.Trim();
            _desc.BaseApiPath = string.IsNullOrEmpty(otherBaseApiPath) ? "" : "/" + otherBaseApiPath.Trim('/');
            return this;
        }

        IRequestWithExpectedStatus IRequestWithUrl.Expecting(params HttpStatusCode[] expectedStatusCodes)
        {
            if(!expectedStatusCodes.Any())
                expectedStatusCodes = new[] { HttpStatusCode.OK };

            _desc.ExpectedStatusCodes = expectedStatusCodes;
            return this;
        }

        IExecutableRequest<T> IRequestWithExpectedStatus.WithContent<T>()
        {
            return new FluentRequestWithContent<T>(_desc);
        }

        void IExecutableRequest.Execute()
        {
            _desc.Request.Resource = _desc.BaseApiPath + _desc.ResourcePath;

            this.TraceRequest();
            _desc.Response = _desc.RestClient.Execute(_desc.Request);
            this.ValidateResponse();        
        }

        Task IExecutableRequest.ExecuteAsync()
        {
            _desc.Request.Resource = _desc.BaseApiPath + _desc.ResourcePath;

            this.TraceRequest();

            var tcs = new TaskCompletionSource<object>();
            _desc.RestClient.ExecuteAsync(_desc.Request, (response, handle) =>
            {
                try
                {
                    _desc.Response = response;
                    this.ValidateResponse();
                    tcs.SetResult(null);
                }
                catch(Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }
    }

    internal class FluentRequestWithContent<T> : FluentRequest, IExecutableRequest<T>
    {
        public FluentRequestWithContent(RequestDescription description)
            : base(description)
        {
        }

        private T DeserializeResponseContent()
        {
            try
            {
                using(var reader = new StringReader(_desc.Response.Content))
                {
                    var jsonReader = new JsonTextReader(reader);
                    return _desc.Serializer.Deserialize<T>(jsonReader);
                }
            }
            catch(Exception ex)
            {
                _desc.TraceError(TraceLevel.Error, ex, "Could not {0} (error while deserializing response): {1}", _desc.Operation, ex.Message);
                throw new RestException("Error while deserializing response: " + ex.Message, ex);
            }
        }

        T IExecutableRequest<T>.Execute()
        {
            ((IExecutableRequest)this).Execute();
            return this.DeserializeResponseContent();
        }

        async Task<T> IExecutableRequest<T>.ExecuteAsync()
        {
            await ((IExecutableRequest)this).ExecuteAsync();
            return this.DeserializeResponseContent();
        }
    }
}