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

        public FeedBackgroundService(ILogger<FeedBackgroundService> logger, ITwitterFeedProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _provider.ReadFromTwitterAsync(stoppingToken);
        }
    }
}