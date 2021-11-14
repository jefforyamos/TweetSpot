using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TweetSpot.ServiceBus.Events;
using TweetSpot.Models;

namespace TweetSpot.ServiceBus.Consumers
{
    public class LogEventsToConsoleConsumer : IConsumer<ITwitterFeedInitStarted>,
            IConsumer<IFeedSpeedReported>,
            IConsumer<ICryptoTweetReceived>
    {
        private readonly ILogger<LogEventsToConsoleConsumer> _logger;

        public LogEventsToConsoleConsumer(ILogger<LogEventsToConsoleConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<ITwitterFeedInitStarted> context)
        {
            var config = context.Message.Configuration;
            _logger.LogInformation(@$"TWITTER Feed provider is opening access to feed using the following settings: 
            Bearer Token: {config.TwitterBearerToken} ({TwitterFeedConfiguration.EnvironmentConfigKeys.TwitterBearerToken})
            Client Timeout: {config.ClientTimeout} ({TwitterFeedConfiguration.EnvironmentConfigKeys.TwitterClientTimeout})
            Speed Report Interval: {config.SpeedReportIntervalCount} ({TwitterFeedConfiguration.EnvironmentConfigKeys.SpeedReportIntervalCount})
            Stream Buffer Size: {config.StreamBufferSizeInKb:N0} KB ({TwitterFeedConfiguration.EnvironmentConfigKeys.StreamBufferSizeInKb})
            Twitter URI: {config.SampledStreamUri} (not configurable, no env value)
");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<IFeedSpeedReported> speedReport)
        {
            _logger.LogInformation($"Current speed: {speedReport.Message.CurrentSpeedTweetsPerSecond:N1}/sec.  Average: {speedReport.Message.AverageSpeedTweetsPerSecond:N1}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ICryptoTweetReceived> context)
        {
            var formattedKeywordList = string.Join(",", context.Message.KeywordsDetected);
            _logger.LogInformation($"Crypto Tweet [{formattedKeywordList}] - {context.Message.UnparsedData}");
            return Task.CompletedTask;
        }
    }
}