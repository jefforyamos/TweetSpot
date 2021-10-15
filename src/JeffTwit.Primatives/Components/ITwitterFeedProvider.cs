using System.Collections.Generic;
using System.Threading;
using JeffTwit.ServiceBus.Commands;

namespace JeffTwit.Components
{
    public interface ITwitterFeedProvider
    {
        IAsyncEnumerable<IProcessIncomingTweet> ReadAsync(CancellationToken cancelToken);
    }
}