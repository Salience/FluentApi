using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

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

        public void SetAuthenticator(IAuthenticator authenticator)
        {
            Guard.NotNull(authenticator, "authenticator");

            _restClient.Authenticator = authenticator;
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

        internal void Trace(TraceLevel level, string messageFormat, params object[] args)
        {
            foreach(var traceWriter in _traceWriters)
                traceWriter.Trace(level, string.Format(messageFormat, args));
        }

        internal void TraceError(TraceLevel level, Exception exception, string messageFormat, params object[] args)
        {
            foreach(var traceWriter in _traceWriters)
                traceWriter.Trace(level, string.Format(messageFormat, args), exception);
        }

        public IRequestWithOperation To(string operation)
        {
            Guard.NotNullOrEmpty(operation, "operation");

            return new FluentRequest(new RequestDescription
            {
                RestClient = _restClient,
                Serializer = this.Serializer,
                Operation = operation,
                BaseApiPath = _defaultBaseApiPath,
                Trace = this.Trace,
                TraceError = this.TraceError
            });
        }
    }
}