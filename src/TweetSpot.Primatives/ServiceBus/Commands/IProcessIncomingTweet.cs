using System;

namespace TweetSpot.ServiceBus.Commands
{
    public interface IProcessIncomingTweet
    {
        ulong Id { get; }

        DateTime ReceivedDateTimeUtc { get; }

        string UnparsedData { get; }

        ulong OrdinalCountNumber { get; }
    }

    public class ProcessIncomingTweet : IProcessIncomingTweet
    {
        //       {"data":{"id":"1450893958457012231","text":"RT @nite_baron: #BCSpoilers waiting meme https://t.co/N6CKWo5CoQ"}}
        private ProcessIncomingTweet(ulong id, string rawTweet, DateTime receivedDateTimeUtc, ulong ordinalCountNumber)
        {
            Id = id;
            UnparsedData = rawTweet;
            ReceivedDateTimeUtc = receivedDateTimeUtc;
            OrdinalCountNumber = ordinalCountNumber;
        }
        public ulong Id { get; }

        public DateTime ReceivedDateTimeUtc { get; }

        public string UnparsedData { get; }

        public ulong OrdinalCountNumber { get; }

        public static ProcessIncomingTweet? Create(string rawTweet, DateTime receivedDateTimeUtc, ulong ordinalCountNumber)
        {
            const int IdStartPosition = 15;
            const int IdLength = 19;

            if (rawTweet.Length < (IdStartPosition + IdLength)) return null; // Too short
            var idStr = rawTweet.Substring(IdStartPosition, IdLength);
            return (ulong.TryParse(idStr, out var idResult))
                ? new ProcessIncomingTweet(idResult, rawTweet, receivedDateTimeUtc, ordinalCountNumber)
                : null;
        }

        public override string ToString()
        {
            return $"Tweet {Id} Ordinal {OrdinalCountNumber:N0}";
        }
    }
}