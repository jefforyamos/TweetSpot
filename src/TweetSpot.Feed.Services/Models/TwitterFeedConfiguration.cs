using Microsoft.Extensions.Configuration;
using System;
using TweetSpot.Exceptions;

namespace TweetSpot.Models
{
    public class TwitterFeedConfiguration : ITwitterFeedConfiguration
    {
        private readonly IConfiguration _configuration;

        // Keys that correlate to environment values
        internal const string TwitterBearerTokenKey = "TwitterBearerToken";
        internal const string TwitterClientTimeoutKey = "TwitterClientTimeout";

        // Defaults
        internal const string SampledStreamUriDefault = "https://api.twitter.com/2/tweets/sample/stream";
        internal static readonly TimeSpan ClientTimeoutDefault = TimeSpan.FromSeconds(20);

        // Max values
        internal static readonly TimeSpan ClientTimeoutMaximumAllowed = TimeSpan.FromMinutes(5);

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
                return token;
            }
        }

        public Uri SampledStreamUri { get; }
        public int SpeedReportIntervalCount => 100; // Todo: Get config from environment?
        public TimeSpan ClientTimeout
        {
            get
            {
                var configValue = _configuration[TwitterClientTimeoutKey];
                if (configValue != null && TimeSpan.TryParse(configValue, out var parsedValue))
                {
                    if (parsedValue <= ClientTimeoutMaximumAllowed) return parsedValue;
                    return int.TryParse(configValue, out var intResult) ? TimeSpan.FromSeconds(intResult) : ClientTimeoutDefault;
                }
                return ClientTimeoutDefault;
            }
        }

        public int? StreamBufferSize => 10 * 1_024;
        public void DemandEssentialSettings()
        {
            if( string.IsNullOrWhiteSpace(_configuration[TwitterBearerTokenKey]))
                throw new EnvironmentConfigurationException(TwitterBearerTokenKey,"Twitter feed cannot be started without a bearer token provided by Twitter.");
        }
    }
}