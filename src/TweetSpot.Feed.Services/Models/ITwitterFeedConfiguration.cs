using System;

namespace TweetSpot.Models
{
    public interface ITwitterFeedConfiguration
    {
        string TwitterBearerToken { get; }

        Uri SampledStreamUri { get; }

        /// <summary>
        /// Send a speed report every Xxx number of incoming records
        /// </summary>
        int SpeedReportIntervalCount { get; }

        TimeSpan ClientTimeout { get; }
    }
}
