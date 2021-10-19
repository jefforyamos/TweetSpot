using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TweetSpot.Components;

namespace TweetSpot.BackgroundServices
{
    public class FeedBackgroundService : BackgroundService
    {
        private readonly ILogger<FeedBackgroundService> _logger;
        private readonly ITwitterFeedProvider _provider;
        private readonly ITwitterFeedConsumer _consumer;

        public FeedBackgroundService(ILogger<FeedBackgroundService> logger, ITwitterFeedProvider provider, ITwitterFeedConsumer consumer)
        {
            _logger = logger;
            _provider = provider;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _consumer.ConsumeAsync(_provider, stoppingToken);
            }
        }
    }
}