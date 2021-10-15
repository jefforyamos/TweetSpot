using System.Collections.Generic;
using System.Threading;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Components
{
    public interface ITwitterFeedProvider
    {
        IAsyncEnumerable<IProcessIncomingTweet> ReadAsync(CancellationToken cancelToken);
    }
}