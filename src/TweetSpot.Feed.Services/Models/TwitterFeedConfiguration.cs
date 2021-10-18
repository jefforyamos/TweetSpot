using System;
using Microsoft.Extensions.Configuration;

namespace TweetSpot.Models
{
    public class TwitterFeedConfiguration : ITwitterFeedConfiguration
    {
        private readonly IConfiguration _configuration;
        private const string TwitterBearerTokenKey = "TwitterBearerToken";

        public TwitterFeedConfiguration(IConfiguration configuration )
        {
            _configuration = configuration;
        }

        public string TwitterBearerToken
        {
            get
            {
                var token = _configuration[TwitterBearerTokenKey];
                if (token == null ) throw new InvalidOperationException($"Invalid configuration.  {TwitterBearerTokenKey} user secret is not set.");
                return token;
            }
        }


    }
}