using System;

namespace TweetSpot.Models
{
    /// <summary>
    /// Defines the data fields available in an unparsed tweet.
    /// </summary>
    public interface IUnparsedTweet
    {
        /// <summary>
        /// The Id value as assigned by Twitter.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// The UTC timestamp at which it was received in our feed.
        /// </summary>
        DateTime ReceivedDateTimeUtc { get; }

        /// <summary>
        /// The unparsed Tweet data that was delivered.
        /// </summary>
        string UnparsedData { get; }

        /// <summary>
        /// The ordinal order in which it was received.  Resets to zero at startup.
        /// </summary>
        ulong OrdinalCountNumber { get; }
    }
}