using TweetSpot.Models;

namespace TweetSpot.ServiceBus.Events
{
    /// <summary>
    /// This represents an event to be published when a tweet is detected that pertains in one way or another to Crypto topics. 
    /// </summary>
    public interface ICryptoTweetReceived : IUnparsedTweet
    {
        string[] KeywordsDetected { get; }
    }
}