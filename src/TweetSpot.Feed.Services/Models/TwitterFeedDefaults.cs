using System;

namespace TweetSpot.Models
{
    public class TwitterFeedDefaults : ITwitterFeedConfiguration
    {
        internal const string SampledStreamUriDefault = "https://api.twitter.com/2/tweets/sample/stream";
        internal static readonly TimeSpan ClientTimeoutDefault = TimeSpan.FromSeconds(20);

        public TwitterFeedDefaults()
        {
            SampledStreamUri = new Uri(SampledStreamUriDefault);
        }

        public string TwitterBearerToken => String.Empty;

        public Uri SampledStreamUri { get; }

        public int SpeedReportIntervalCount => 100;

        public TimeSpan ClientTimeout => TimeSpan.FromSeconds(20);

        public int StreamBufferSizeInKb => 10; // 10K default
    }
}