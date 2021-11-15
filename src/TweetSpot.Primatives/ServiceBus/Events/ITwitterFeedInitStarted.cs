using TweetSpot.Models;

namespace TweetSpot.ServiceBus.Events
{
    public interface ITwitterFeedInitStarted
    {
        /// <summary>
        /// The configuration being used as the feed starts up.
        /// </summary>
        ITwitterFeedConfiguration Configuration { get; }
    }
}