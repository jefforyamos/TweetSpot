using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace TweetSpot.ServiceBus.Commands
{
    /// <summary>
    /// Actual implementation of the command-verb that is instanced in the Feed.
    /// </summary>
    [Table("Tweets")]
    public class IncomingTweet : IProcessIncomingTweet
    {
        //       {"data":{"id":"1450893958457012231","text":"RT @nite_baron: #BCSpoilers waiting meme https://t.co/N6CKWo5CoQ"}}
        private IncomingTweet(ulong id, string rawTweet, DateTime receivedDateTimeUtc, ulong ordinalCountNumber)
        {
            Id = id;
            UnparsedData = rawTweet;
            ReceivedDateTimeUtc = receivedDateTimeUtc;
            OrdinalCountNumber = ordinalCountNumber;
        }

        public IncomingTweet()
        {
            UnparsedData = string.Empty;
        }
        public ulong Id { get; }

        public DateTime ReceivedDateTimeUtc { get; }

        public string UnparsedData { get; }

        public ulong OrdinalCountNumber { get; }

        public static IncomingTweet? Create(string rawTweet, DateTime receivedDateTimeUtc, ulong ordinalCountNumber)
        {
            const int idStartPosition = 15;
            const int idLength = 19;

            if (rawTweet.Length < (idStartPosition + idLength)) return null; // Too short
            Debug.Assert(rawTweet.Substring(0, 15) == "{\"data\":{\"id\":\"", "Beginning of tweet seems malformed, bad test data?");
            var idStr = rawTweet.Substring(idStartPosition, idLength);
            Debug.Assert(rawTweet.Substring(35, 1) == ",", "The id data doesn't seem to be the right length, bad test data?");
            return (ulong.TryParse(idStr, out var idResult))
                ? new IncomingTweet(idResult, rawTweet, receivedDateTimeUtc, ordinalCountNumber)
                : null;
        }

        public override string ToString()
        {
            return $"Tweet {Id} Ordinal {OrdinalCountNumber:N0}";
        }
    }
}