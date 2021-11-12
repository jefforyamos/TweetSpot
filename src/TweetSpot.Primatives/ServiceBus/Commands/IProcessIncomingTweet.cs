using TweetSpot.Models;

namespace TweetSpot.ServiceBus.Commands
{
    public interface IProcessIncomingTweet : IUnparsedTweet
    {
        // No additional fields necessary.
    }
}