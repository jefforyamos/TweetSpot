using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

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
                // _logger.LogInformation(item.ToString());
            }
        }
    }
}