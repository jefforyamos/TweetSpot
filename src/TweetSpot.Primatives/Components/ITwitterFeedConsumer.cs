using System.Threading;
using System.Threading.Tasks;

namespace TweetSpot.Components
{
    public interface ITwitterFeedConsumer
    {
        Task ConsumeAsync(ITwitterFeedProvider provider, CancellationToken cancelToken);
    }
}