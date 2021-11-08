using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Persistence
{
    public interface ITweetPersistence
    {
        ulong Add(IncomingTweet tweet);

        IncomingTweet Get(ulong id);
    }
}