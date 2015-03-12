using System;
using System.Net;
using RestSharp;

namespace Salience.FluentApi
{
    internal delegate void TraceMethod(TraceLevel level, string messageFormat, params object[] args);
    internal delegate void TraceErrorMethod(TraceLevel level, Exception error, string messageFormat, params object[] args);

    internal class RequestDescription
    {
        public RestClient RestClient { get; set; }
        public TraceMethod Trace { get; set; }
        public TraceErrorMethod TraceError { get; set; }
        public Newtonsoft.Json.JsonSerializer Serializer { get; set; }
        public RestRequest Request { get; set; }
        public string Operation { get; set; }
        public string BaseApiPath { get; set; }
        public string ResourcePath { get; set; }
        public HttpStatusCode[] ExpectedStatusCodes { get; set; }
        public IRestResponse Response { get; set; }
    }
}