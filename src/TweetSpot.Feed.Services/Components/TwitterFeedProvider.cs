using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TweetSpot.Models;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        private readonly ILogger<TwitterFeedProvider> _logger;
        private readonly ITwitterFeedConfiguration _configuration;

        public TwitterFeedProvider(ILogger<TwitterFeedProvider> logger, ITwitterFeedConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async IAsyncEnumerable<IProcessIncomingTweet> ReadAsync([EnumeratorCancellation] CancellationToken cancelToken)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new FakeTweet {id = i};
                await Task.Delay(1000, cancelToken);
            }
        }

        private class FakeTweet : ServiceBus.Commands.IProcessIncomingTweet
        {
            public int id { get; set; }

        }
        
    }
}