using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TweetSpot.ServiceBus.Events;

namespace TweetSpot.AllInOne.ServiceBus.Consumers
{
    public class LogEventsToConsoleConsumer : IConsumer<ITwitterFeedInitStarted>
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
    }
}