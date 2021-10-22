using System;

namespace TweetSpot.ServiceBus.Events
{
    /// <summary>
    /// Used to publish each report.  Internalizes most of the calculations between cycles.
    /// </summary>
    public class FeedSpeedReported : IFeedSpeedReported
    {
        /// <summary>
        /// Create a new <see cref="IFeedSpeedReported"/> event.
        /// </summary>
        /// <param name="elapsedTime">Current elapsed time value on the timer</param>
        /// <param name="totalCount">Total count of tweets received on the feed since startup</param>
        /// <param name="previousReport">The previous report so current speed can be computed, or null if this is the first cycle</param>
        public FeedSpeedReported(TimeSpan elapsedTime, ulong totalCount, FeedSpeedReported? previousReport)
        {
            TimeSpanForAllCycles = elapsedTime;
            TweetsReceivedForAllCycles = totalCount;
            AverageSpeedTweetsPerSecond = Convert.ToDouble(TweetsReceivedForAllCycles) / TimeSpanForAllCycles.TotalSeconds;
            if (previousReport == null)
            {
                TweetsReceivedThisCycle = TweetsReceivedForAllCycles;
                CurrentSpeedTweetsPerSecond = AverageSpeedTweetsPerSecond;
                TweetsReceivedThisCycle = TweetsReceivedForAllCycles;
            }
            else
            {
                TweetsReceivedThisCycle = totalCount - previousReport.TweetsReceivedForAllCycles;
                TimeSpanThisCycle = TimeSpanForAllCycles.Subtract(previousReport.TimeSpanForAllCycles);
                CurrentSpeedTweetsPerSecond = Convert.ToDouble(TweetsReceivedThisCycle) / TimeSpanThisCycle.TotalSeconds;
            }
        }
        public ulong TweetsReceivedThisCycle { get; }
        public TimeSpan TimeSpanThisCycle { get; }
        public double CurrentSpeedTweetsPerSecond { get; }
        public ulong TweetsReceivedForAllCycles { get; }
        public TimeSpan TimeSpanForAllCycles { get; }
        public double AverageSpeedTweetsPerSecond { get; }
    }
}