using System;
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

    }

    public interface IDemandEssentialSettingsOnStartup
    {
        void DemandEssentialSettings();
    }

}
