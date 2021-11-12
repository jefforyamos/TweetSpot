using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TweetSpot.ServiceBus.Events;

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
            _logger.LogInformation($"TWITTER Feed provider is opening access to feed using token {context.Message.BearerTokenAbbreviation}.");
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