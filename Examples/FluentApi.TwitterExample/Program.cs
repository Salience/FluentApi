using System;
using RestSharp;
using Salience.FluentApi;

namespace FluentApi.TwitterExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use Application-only authentication to obtain a bearer token
            var oauthClient = new FluentClient("https://api.twitter.com");
            //oauthClient.AddTrace(new ConsoleTraceWriter()); // uncomment to see requests in console
            oauthClient.SetAuthenticator(new HttpBasicAuthenticator("<yourApiKey>", "<yourSecret>"));
            
            var obtainBearerTokenRequest = oauthClient
                .To("obtain a bearer token")
                .Post("oauth2/token", r => r
                    .AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost))
                .Expecting((dynamic response) => (string)response.access_token);

            var accessToken = obtainBearerTokenRequest.Execute();

            // Authenticate API requests with the bearer token
            var apiClient = new FluentClient("https://api.twitter.com", "/1.1");
            apiClient.SetAuthenticator(
                new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer"));

            // Twitter use underscores but we love PascalCase
            apiClient.Serializer.ContractResolver =
                new UnderscorePropertyNamesContractResolver();

            // Use the authenticated client to perform requests
            var searchTweetsRequest = apiClient
                .To("search for tweets")
                .Get("search/tweets.json", r => r
                    .AddParameter("q", "@twitterapi")
                    .AddParameter("result_type", "recent")
                    .AddParameter("count", 50))
                .Expecting((TwitterSearchResult result) => result.Statuses);

            var tweets = searchTweetsRequest.Execute();
            Console.WriteLine("=== RESULTS ===");
            foreach(var tweet in tweets)
            {
                Console.WriteLine("@{0}: \"{1}\" at {2}{3}", 
                    tweet.User.ScreenName, tweet.Text, tweet.CreatedAt, Environment.NewLine);
            }

            Console.ReadLine();
        }

        class TwitterUser
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string ScreenName { get; set; }
            public string Description { get; set; }

            public int FollowersCount { get; set; }
            public int FriendsCount { get; set; }
        }

        class TwitterStatus
        {
            public string CreatedAt { get; set; }

            public long Id { get; set; }
            public TwitterUser User { get; set; }
            public string Text { get; set; }

            public bool Favorited { get; set; }
            public bool Retweeted { get; set; }
        }

        class TwitterSearchResult
        {
            public TwitterStatus[] Statuses { get; set; }
        }
    }
}
