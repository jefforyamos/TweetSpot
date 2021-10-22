using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Components
{
    public interface ITwitterFeedProvider
    {
        Task ReadFromTwitterAsync(CancellationToken cancelToken);
    }
}