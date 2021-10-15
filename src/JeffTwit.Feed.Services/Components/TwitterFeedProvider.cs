using System.Collections.Generic;
using System.Threading;
using JeffTwit.ServiceBus.Commands;

namespace JeffTwit.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        public IAsyncEnumerable<IProcessIncomingTweet> ReadAsync(CancellationToken cancelToken)
        {
            throw new System.NotImplementedException();
        }
    }
}