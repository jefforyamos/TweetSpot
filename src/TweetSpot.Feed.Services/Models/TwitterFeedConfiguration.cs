using Microsoft.Extensions.Configuration;
using System;
using TweetSpot.Exceptions;

namespace TweetSpot.Models
{
    public class TwitterFeedConfiguration : ITwitterFeedConfiguration, IDemandEssentialSettingsOnStartup
    {
        private readonly IConfiguration _configuration;

        public static class EnvironmentConfigKeys
        {
            public const string TwitterBearerToken = nameof(TwitterBearerToken);
            public const string TwitterClientTimeout = nameof(TwitterClientTimeout);
            public const string StreamBufferSizeInKb = nameof(StreamBufferSizeInKb);
            public const string SpeedReportIntervalCount = nameof(SpeedReportIntervalCount);
        }

        // Defaults
        public static readonly ITwitterFeedConfiguration Defaults = new TwitterFeedDefaults();

        // Max values
        internal static readonly TimeSpan ClientTimeoutMaximumAllowed = TimeSpan.FromMinutes(5);

        public TwitterFeedConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            SampledStreamUri = Defaults.SampledStreamUri;
            TwitterBearerToken = _configuration[EnvironmentConfigKeys.TwitterBearerToken] ?? Defaults.TwitterBearerToken;
            ClientTimeout = DetermineClientTimeout(configuration) ?? Defaults.ClientTimeout;
            StreamBufferSizeInKb = DetermineStreamBufferSize(configuration) ?? Defaults.StreamBufferSizeInKb;
            SpeedReportIntervalCount = DetermineSpeedReportIntervalCount(configuration) ?? Defaults.SpeedReportIntervalCount;
        }

        internal static TimeSpan? DetermineClientTimeout(IConfiguration configuration)
        {
            var configValue = configuration[EnvironmentConfigKeys.TwitterClientTimeout];
            if (configValue != null && TimeSpan.TryParse(configValue, out var parsedValue))
            {
                if (parsedValue <= ClientTimeoutMaximumAllowed) return parsedValue;
                if ( int.TryParse(configValue, out var intResult) ) return TimeSpan.FromSeconds(intResult);
            }
            return default;
        }

        internal static int? DetermineStreamBufferSize(IConfiguration configuration)
        {
            var configValue = configuration[EnvironmentConfigKeys.StreamBufferSizeInKb];
            if(configValue != null && int.TryParse(configValue, out var intResult) )
            {
                return intResult;
            }
            return default;
        }

        internal static int? DetermineSpeedReportIntervalCount(IConfiguration configuration)
        {
            var configValue = configuration[EnvironmentConfigKeys.SpeedReportIntervalCount];
            if (configValue != null && int.TryParse(configValue, out var intResult))
            {
                return intResult;
            }
            return default;
        }

        public string TwitterBearerToken { get; }

        public Uri SampledStreamUri { get; }
        public int SpeedReportIntervalCount { get; }
        public TimeSpan ClientTimeout { get; }

        public int StreamBufferSizeInKb { get; }

        public void DemandEssentialSettings()
        {
            if (string.IsNullOrWhiteSpace(TwitterBearerToken))
                throw new EnvironmentConfigurationException(EnvironmentConfigKeys.TwitterBearerToken, "Twitter feed cannot be started without a bearer token provided by Twitter.");

        }
    }
}