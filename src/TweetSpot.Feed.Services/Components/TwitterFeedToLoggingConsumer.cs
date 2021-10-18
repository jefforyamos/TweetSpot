using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TweetSpot.Components
{
    public class TwitterFeedToLoggingConsumer : ITwitterFeedConsumer
    {
        private readonly ILogger<TwitterFeedToLoggingConsumer> _logger;

        public TwitterFeedToLoggingConsumer(ILogger<TwitterFeedToLoggingConsumer> logger)
        {
            _logger = logger;
        }
        public async Task ConsumeAsync(ITwitterFeedProvider provider, CancellationToken cancelToken)
        {
            await foreach (var item in provider.ReadAsync(cancelToken))
            {
                _logger.LogInformation($"Tweet #{item.id}");
            }
        }
    }
}