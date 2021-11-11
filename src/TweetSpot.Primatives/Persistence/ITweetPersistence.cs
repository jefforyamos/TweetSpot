using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Persistence
{
    /// <summary>
    /// Defines the interface required to persist tweet content to a data store.
    /// </summary>
    public interface ITweetPersistence
    {
        /// <summary>
        /// Add a tweet.  If it already exists, it is updated.
        /// </summary>
        /// <param name="tweet">Tweet data to be persisted.</param>
        /// <returns>The Id number that was used in the data store, should be same as Twitter.</returns>
        ulong Add(IncomingTweet tweet);

        /// <summary>
        /// Gets a specified tweet.
        /// </summary>
        /// <param name="id">The id number of the persisted value.</param>
        /// <returns></returns>
        IncomingTweet Get(ulong id);
    }
}