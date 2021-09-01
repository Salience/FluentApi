using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using Salience.FluentApi.Internal;

namespace Salience.FluentApi
{
    public class FluentClient : IFluentClient
    {
        private readonly RestClient _restClient;
        private readonly string _defaultBaseApiPath;
        private readonly List<ITraceWriter> _traceWriters = new List<ITraceWriter>();

        public JsonSerializer Serializer { get; set; }

        public static JsonSerializer GetNewDefaultSerializer()
        {
            return new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateParseHandling = DateParseHandling.None
            };
        }

        public FluentClient(string host, string defaultBaseApiPath = "")
        {
            Guard.NotNullOrEmpty(host, "host");
            Guard.NotNull(defaultBaseApiPath, "defaultBaseApiPath");

            _defaultBaseApiPath = defaultBaseApiPath.Trim();
            _defaultBaseApiPath = string.IsNullOrEmpty(_defaultBaseApiPath) ? "" : "/" + _defaultBaseApiPath.Trim('/');
            var url = new Uri(host.Trim().TrimEnd('/'), UriKind.Absolute);
            _restClient = new RestClient { BaseUrl = url };

            this.Serializer = GetNewDefaultSerializer();
        }

        public IFluentClient SetAuthenticator(IAuthenticator authenticator)
        {
            Guard.NotNull(authenticator, "authenticator");

            _restClient.Authenticator = authenticator;
            return this;
        }

        public IFluentClient AddTrace(ITraceWriter traceWriter)
        {
            Guard.NotNull(traceWriter, "traceWriter");

            _traceWriters.Add(traceWriter);
            return this;
        }

        public IFluentClient RemoveTrace(ITraceWriter traceWriter)
        {
            Guard.NotNull(traceWriter, "traceWriter");

            _traceWriters.Remove(traceWriter);
            return this;
        }

        public IRequestWithOperation To(string operation)
        {
            var data = this.CreateRequestData();
            var request = this.CreateEmptyRequest(data);
            return request.To(operation);
        }

        protected internal virtual RequestData CreateRequestData()
        {
            return new RequestData
            {
                BaseApiPath = _defaultBaseApiPath,
                ExpectedStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.OK },
                AlternateResults = new Dictionary<HttpStatusCode, object>()
            };
        }

        protected internal virtual IEmptyRequest CreateEmptyRequest(RequestData data)
        {
            return new FluentRequest(this, data);
        }

        protected internal virtual void Trace(TraceLevel level, string messageFormat, params object[] args)
        {
            try
            {
                foreach(var traceWriter in _traceWriters)
                    traceWriter.Trace(level, null, messageFormat, args);
            }
            catch(Exception) { }
        }

        protected internal virtual void TraceError(TraceLevel level, Exception exception, string messageFormat, params object[] args)
        {
            try
            {
                foreach(var traceWriter in _traceWriters)
                    traceWriter.Trace(level, exception, messageFormat, args);
            }
            catch(Exception) { }
        }

        protected internal virtual object HandleRequest(RequestData data)
        {
            this.CreateRestRequest(data);
            this.ConfigureRequest(data);
            this.TraceRequest(data);
            this.ExecuteRequest(data);
            this.ValidateResponse(data);
            this.TraceResponse(data);

            var result = this.GetResultFromResponse(data);

            foreach(var followup in data.FollowUps)
            {
                var nextRequest = followup(result);
                result = nextRequest.Execute();
            }

            return result;
        }

        protected internal virtual async Task<object> HandleRequestAsync(RequestData data, System.Threading.CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            this.CreateRestRequest(data);
            this.ConfigureRequest(data);
            this.TraceRequest(data);
            await this.ExecuteRequestAsync(data);
            this.ValidateResponse(data);
            this.TraceResponse(data);

            var result = this.GetResultFromResponse(data);

            foreach (var followup in data.FollowUps)
            {
                token.ThrowIfCancellationRequested();

                var nextRequest = followup(result);
                result = await nextRequest.ExecuteAsync();
            }

            return result;
        }

        protected internal virtual void CreateRestRequest(RequestData data)
        {
            data.Request = new RestRequest
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonDotNetSerializerWrapper(this.Serializer)
            };
            data.Request.AddHeader("Accept", "application/json, text/json, text/x-json, text/javascript");
        }

        protected internal virtual void ConfigureRequest(RequestData data)
        {
            try
            {
                data.Request.Resource = data.BaseApiPath + data.ResourcePath;
                data.Request.Method = data.Method;
                data.RequestCustomizer?.Invoke(data.Request);

                if (data.Method == Method.POST || data.Method == Method.PUT || data.Method == Method.PATCH)
                    if(!data.Request.Parameters.Any(p => p.Type == ParameterType.HttpHeader && p.Name == "Content-Type"))
                        data.Request.AddHeader("Content-Type", "application/json");
            }
            catch(Exception ex)
            {
                this.TraceError(TraceLevel.Error, ex, "Could not {0} (error while creating request): {1}", data.Operation, ex.Message);
                throw new RestException("Error while creating request: " + ex.Message, ex);
            }
        }

        protected internal virtual void TraceRequest(RequestData data)
        {
            var requestUri = _restClient.BuildUri(data.Request);
            var requestBodyParameter = data.Request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            var requestBodyContent = requestBodyParameter == null ? "(no body)" : requestBodyParameter.Value;
            this.Trace(TraceLevel.Debug, "Trying to {0} by sending {1} request to {2} : {3}", data.Operation, data.Request.Method, requestUri, requestBodyContent);
        }

        protected internal virtual void ExecuteRequest(RequestData data)
        {
            data.Response = _restClient.Execute(data.Request);
        }

        protected internal virtual async Task ExecuteRequestAsync(RequestData data)
        {
            data.Response = await _restClient.ExecuteAsync(data.Request);
        }

        protected internal virtual void ValidateResponse(RequestData data)
        {
            var response = data.Response;

            // check transport error
            if(response.ErrorException != null)
            {
                this.TraceError(TraceLevel.Error, response.ErrorException, "Could not {0} (transport level error): {1}", data.Operation, response.ErrorMessage);
                throw new RestException("Transport level error: " + response.ErrorMessage, response.ErrorException);
            }

            // check status
            if(data.ExpectedStatusCodes.Contains(response.StatusCode))
                return;

            if(200 <= (int)response.StatusCode && (int)response.StatusCode < 300)
                return;

            string responseContent = response.Content ?? "(no content)";
            this.TraceError(TraceLevel.Error, response.ErrorException, "Could not {0} (wrong status returned - {1}): {2}", data.Operation, response.StatusCode, responseContent);

            this.HandleUnexpectedResponse(data);
            
            throw new RestException("Wrong status returned: " + response.StatusDescription, response.Content, response.StatusCode);
        }

        protected internal virtual void HandleUnexpectedResponse(RequestData data)
        {
            // by default, do nothing
        }

        protected internal virtual void TraceResponse(RequestData data)
        {
            this.Trace(TraceLevel.Debug, "Received response to {0}: {1}", data.Operation, data.Response.Content ?? "(no content)");
        }

        protected internal virtual object GetResultFromResponse(RequestData data)
        {
            if(data.UseRawResponseContent)
                return (data.ResultGetter != null ? data.ResultGetter.DynamicInvoke(data.Response.Content) : data.Response.Content);

            if (data.ResponseBodyType == null)
                return null;

            if(data.AlternateResults.ContainsKey(data.Response.StatusCode))
                return data.AlternateResults[data.Response.StatusCode];

            try
            {
                using(var reader = new StringReader(data.Response.Content))
                {
                    var jsonReader = new JsonTextReader(reader);

                    var responseBody = this.Serializer.Deserialize(jsonReader, data.ResponseBodyType);
                    return (data.ResultGetter != null ? data.ResultGetter.DynamicInvoke(responseBody) : responseBody);
                }
            }
            catch(Exception ex)
            {
                this.TraceError(TraceLevel.Error, ex, "Could not {0} (error while deserializing response): {1}", data.Operation, ex.Message);
                throw new RestException("Error while deserializing response: " + ex.Message, ex);
            }
        }
    }
}