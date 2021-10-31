using TweetSpot.ServiceBus.Commands;
using MassTransit;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TweetSpot.ServiceBus.Consumers
{
    public class ProcessIncomingTweetConsumer : IConsumer<IProcessIncomingTweet>
    {
        private readonly IBus _bus;
        private readonly ILogger<ProcessIncomingTweetConsumer> _logger;

        public ProcessIncomingTweetConsumer(IBus bus, ILogger<ProcessIncomingTweetConsumer> logger)
        {
            _bus = bus;
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IProcessIncomingTweet> context)
        {
            // _logger.LogInformation($"Tweet {context.Message.UnparsedData}");
            return Task.CompletedTask;
        }
    }
}