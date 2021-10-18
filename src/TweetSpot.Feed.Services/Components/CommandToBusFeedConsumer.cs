using System.Threading;
using System.Threading.Tasks;

namespace TweetSpot.Components
{
    public class CommandToBusFeedConsumer : ITwitterFeedConsumer
    {
        public Task ConsumeAsync(ITwitterFeedProvider provider, CancellationToken cancelToken)
        {
            throw new System.NotImplementedException();
        }
    }
}