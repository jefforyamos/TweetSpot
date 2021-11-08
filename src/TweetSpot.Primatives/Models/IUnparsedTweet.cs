using System;

namespace TweetSpot.Models
{
    /// <summary>
    /// Defines the data fields available in an unparsed tweet.
    /// </summary>
    public interface IUnparsedTweet
    {
        ulong Id { get; }

        DateTime ReceivedDateTimeUtc { get; }

        string UnparsedData { get; }

        ulong OrdinalCountNumber { get; }
    }
}