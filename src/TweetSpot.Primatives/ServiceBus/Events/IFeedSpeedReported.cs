using System;

namespace TweetSpot.ServiceBus.Events
{
    public interface IFeedSpeedReported
    {
        //  _logger.LogInformation($"{ordinalCount:N0} tweets received in {elapsedSeconds:N1} seconds = {countPerSecond:N1}/sec.  Current speed = {currentSpeed:N1}/sec.");
        ulong TweetsReceivedThisCycle { get; }
        TimeSpan TimeSpanThisCycle { get; }
        double CurrentSpeedTweetsPerSecond { get; }

        ulong TweetsReceivedForAllCycles { get; }
        TimeSpan TimeSpanForAllCycles { get; }

        double AverageSpeedTweetsPerSecond { get; }
    }
}