using System;
using TweetSpot.BackgroundServices;
using TweetSpot.Exceptions;

namespace TweetSpot.Models
{
    /// <summary>
    /// Defines the properties necessary for the <see cref="FeedBackgroundService"/> to properly function./>
    /// </summary>
    public interface ITwitterFeedConfiguration
    {
        /// <summary>
        /// The security token provided by Twitter for authentication.
        /// </summary>
        string TwitterBearerToken { get; }

        /// <summary>
        /// Returns a displayable version of the bearer token to confirm the value being used without disclosing the value in it's entirety.
        /// </summary>
        string TwitterBearerTokenAbbreviation
        {
            get
            {
                const int minimumLength = 10;
                var token = TwitterBearerToken;
                var length = token?.Length ?? 0;
                if (length < minimumLength) return "<BAD TOKEN>";
                var beginSegment = token?.Substring(0, 3) ?? string.Empty;
                var endSegment = token?.Substring(length - 3, 3) ?? string.Empty;
                return $"{beginSegment}...{endSegment}";
            }
        }

        /// <summary>
        /// The Uri that should be called by the service
        /// </summary>
        Uri SampledStreamUri { get; }

        /// <summary>
        /// Send a speed report every Xxx number of incoming records
        /// </summary>
        int SpeedReportIntervalCount { get; }

        /// <summary>
        /// How long should the client wait for a response from the service before timing out
        /// </summary>
        TimeSpan ClientTimeout { get; }

        /// <summary>
        /// Size of the buffer to be used to smooth bursts of data from the service.
        /// </summary>
        int? StreamBufferSize { get; }

        /// <summary>
        /// Examines the configuration throwing a fatal exception identifying missing settings.  To be called at startup.
        /// </summary>
        /// <exception cref="EnvironmentConfigurationException">Identifies the name of the first setting that was missing from the environment.</exception>
        void DemandEssentialSettings();
    }
}
