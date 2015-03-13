# FluentApi
A simple wrapper around RestSharp and Json.NET, designed to ease the writing of requests to REST APIs while taking care of errors management and tracing.

Just describe how your requests should be built and let the client take care of the rest!

## Example of usage (based on Twitter API):

```csharp
class Program
{
    static void Main(string[] args)
    {
        // Use Application-only authentication to obtain a bearer token
        var oauthClient = new FluentClient("https://api.twitter.com", "/oauth2");
        oauthClient.AddTrace(new ConsoleTraceWriter());
        oauthClient.SetAuthenticator(new HttpBasicAuthenticator("<yourApiKey>", "<yourSecret>"));
            
        var obtainBearerRequest = oauthClient
            .To("obtain a bearer token")
            .Post("token", r => r.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost))
            .Expecting<dynamic>();

        var bearerResponse = obtainBearerRequest.Execute();
        string accessToken = bearerResponse.access_token;

        // Authenticate API requests with the bearer token
        var apiClient = new FluentClient("https://api.twitter.com", "/1.1");
        apiClient.SetAuthenticator(new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer"));

        // Use the authenticated client to perform requests
        var searchRequest = apiClient
            .To("search for tweets")
            .Get("search/tweets.json", r => r
                .AddParameter("q", "@twitterapi")
                .AddParameter("result_type", "recent")
                .AddParameter("count", 50))
            .Expecting<TwitterSearchResult>();

        var searchResult = searchRequest.Execute();
        Console.WriteLine("=== RESULTS ===");
        foreach(var tweet in searchResult.Statuses)
            Console.WriteLine("{0}: \"{1}\"", tweet.User.Name, tweet.Text);

        Console.ReadLine();
    }

    class TwitterUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Screen_name { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }

        public int Followers_count { get; set; }
        public int Friends_count { get; set; }
    }

    class TwitterStatus
    {
        public string Created_at { get; set; }

        public long Id { get; set; }
        public TwitterUser User { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public string Lang { get; set; }

        public bool Favorited { get; set; }
        public bool Retweeted { get; set; }
    }

    class TwitterSearchResult
    {
        public TwitterStatus[] Statuses { get; set; }
    }

    class ConsoleTraceWriter : ITraceWriter
    {
        public void Trace(TraceLevel level, string message, Exception exception = null)
        {
            Console.WriteLine("[{0}] {1}", level, message);
        }
    }
}
```