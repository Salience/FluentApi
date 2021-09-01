using System;
using System.Collections.Generic;
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
        public HashSet<HttpStatusCode> ExpectedStatusCodes { get; set; }
        public RestRequest Request { get; set; }

        public bool UseRawResponseContent { get; set; }
        public Type ResponseBodyType { get; set; }
        public IRestResponse Response { get; set; }
        public Dictionary<HttpStatusCode, object> AlternateResults { get; set; }
        public Delegate ResultGetter { get; set; }

        public List<FollowUpRequestProvider> FollowUps { get; set; } = new List<FollowUpRequestProvider>();
    }

    public delegate IFinalExecutableRequest<object> FollowUpRequestProvider(object previousResult);
}