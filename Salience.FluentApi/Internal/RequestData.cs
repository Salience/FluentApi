using System;
using System.Net;
using RestSharp;

namespace Salience.FluentApi.Internal
{
    public class RequestData
    {
        public string Operation { get; set; }
        public string BaseApiPath { get; set; }
        public string ResourcePath { get; set; }
        public Method Method { get; set; }
        public Action<RestRequest> RequestCustomizer { get; set; }
        public HttpStatusCode[] ExpectedStatusCodes { get; set; }
        public Type ResponseBodyType { get; set; }
        public Delegate ResultGetter { get; set; }

        public RestRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public object Result { get; set; }
    }
}