using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetSpot.Persistence;
using TweetSpot.ServiceBus.Commands;
using TweetSpot.ServiceBus.Events;

namespace TweetSpot.ServiceBus.Consumers
{
    public class CryptoTweetDetector : IConsumer<IProcessIncomingTweet>
    {
        private readonly IBus _bus;
        private readonly ICryptoKeywordPersistence _keywordPersistence;

        public CryptoTweetDetector(IBus bus, ICryptoKeywordPersistence keywordPersistence)
        {
            _bus = bus;
            _keywordPersistence = keywordPersistence;
        }

        public async Task Consume(ConsumeContext<IProcessIncomingTweet> context)
        {
            var keywordsFound = GetKeywordsFromTweet(context.Message.UnparsedData).ToArray();
            if (keywordsFound.Length > 0)
            {
                var receivedEvent = new CryptoTweetReceived(context.Message, keywordsFound);
                await _bus.Publish<ICryptoTweetReceived>(receivedEvent, context.CancellationToken);
            }
        }

        internal IEnumerable<string> GetKeywordsFromTweet(string rawTweet)
        {
            // Let the chosen persistence class determine how much to cache the keywords, do not cache them here
            foreach (var keyWord in _keywordPersistence.GetKeywords())
            {
                // Not really necessary to JSON parse the tweet for this presuming there's no clash between keywords and JSON field names
                if (rawTweet.Contains(keyWord, StringComparison.InvariantCultureIgnoreCase)) yield return keyWord;
            }
        }

        private class CryptoTweetReceived : ICryptoTweetReceived
        {
            public CryptoTweetReceived(IProcessIncomingTweet tweet, string[] keywordsFound)
            {
                Id = tweet.Id;
                ReceivedDateTimeUtc = tweet.ReceivedDateTimeUtc;
                UnparsedData = tweet.UnparsedData;
                OrdinalCountNumber = tweet.OrdinalCountNumber;
                KeywordsDetected = keywordsFound;
            }
            public ulong Id { get; }
            public DateTime ReceivedDateTimeUtc { get; }
            public string UnparsedData { get; }
            public ulong OrdinalCountNumber { get; }
            public string[] KeywordsDetected { get; }
        }


    }
}
