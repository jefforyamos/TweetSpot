using Microsoft.Extensions.Configuration;
using System;

namespace TweetSpot.Models
{
    public class TwitterFeedConfiguration : ITwitterFeedConfiguration
    {
        private readonly IConfiguration _configuration;
        private const string TwitterBearerTokenKey = "TwitterBearerToken";
        private const string SampledStreamUriDefault = "https://api.twitter.com/2/tweets/sample/stream";

        public TwitterFeedConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            SampledStreamUri = new Uri(SampledStreamUriDefault);
        }

        public string TwitterBearerToken
        {
            get
            {
                var token = _configuration[TwitterBearerTokenKey];
                if (token == null) throw new InvalidOperationException($"Invalid configuration.  {TwitterBearerTokenKey} user secret is not set.");
                return token;
            }
        }

        public Uri SampledStreamUri { get; }
    }
}