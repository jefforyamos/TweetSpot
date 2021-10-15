using System.Collections.Generic;
using System.Threading;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        public IAsyncEnumerable<IProcessIncomingTweet> ReadAsync(CancellationToken cancelToken)
        {
            throw new System.NotImplementedException();
        }
    }
}