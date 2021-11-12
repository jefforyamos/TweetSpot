using MassTransit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TweetSpot.Persistence;
using TweetSpot.ServiceBus.Commands;
using TweetSpot.ServiceBus.Events;
using Xunit;

#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace TweetSpot.ServiceBus.Consumers
{
    public class CryptoTweetDetectorTesting
    {

        private Mock<IBus>? _busMock;
        private CryptoTweetDetector? _serviceBeingTested;
        private ICryptoTweetReceived? _eventThatWasPublishedToTheBus;
        private Mock<ConsumeContext<IProcessIncomingTweet>>? _contextOfEventConsumed;
        private Mock<IProcessIncomingTweet>? _tweetMock;
        private Mock<ICryptoKeywordPersistence>? _persistenceMock;
        private string[] _standardKeywords = new[] { "One", "Two", "Three" };

        private async Task DoSetup(bool publishExpected, string tweetContent, string[] keyWords)
        {
            _persistenceMock = new Mock<ICryptoKeywordPersistence>();
            _persistenceMock.Setup(p => p.GetKeywords()).Returns(keyWords);
            _tweetMock = new Mock<IProcessIncomingTweet>();
            _tweetMock.Setup(tweet => tweet.UnparsedData).Returns(tweetContent);
            _tweetMock.Setup(tweet => tweet.Id).Returns(12345);
            _tweetMock.Setup(tweet => tweet.OrdinalCountNumber).Returns(234);
            _tweetMock.Setup(tweet => tweet.ReceivedDateTimeUtc).Returns(DateTime.UtcNow);
            _contextOfEventConsumed = new Mock<ConsumeContext<IProcessIncomingTweet>>();
            _contextOfEventConsumed.Setup(m => m.Message).Returns(_tweetMock.Object);
            _busMock = new Mock<IBus>();
            if (publishExpected)
            {
                _busMock.Setup(bus => bus.Publish<ICryptoTweetReceived>(It.IsAny<ICryptoTweetReceived>(), It.IsAny<CancellationToken>()));
            }
            _serviceBeingTested = new CryptoTweetDetector(_busMock.Object, _persistenceMock.Object);
            Assert.NotNull(_contextOfEventConsumed?.Object);
            await _serviceBeingTested.Consume(_contextOfEventConsumed.Object);
            _eventThatWasPublishedToTheBus = _busMock.Invocations.Count > 0
                ? _busMock?.Invocations[0]?.Arguments?[0] as ICryptoTweetReceived
                : null;
        }

        [Fact]
        public async Task Consume_OneKeywordMatches_EventGetsPublished()
        {
            await DoSetup(true, "Simulated tweet data includes ONE word that should be detected.", _standardKeywords);
            _busMock?.VerifyAll();
        }

        [Fact]
        public async Task Consume_NoMatches_EventIsNOTPublished()
        {
            await DoSetup(false, "Here is a tweet about something else.", _standardKeywords);
            _busMock?.VerifyAll();
        }

        [Fact]
        public async Task Consume_TwoKeywordsMatch_EventReflectsBothKeywords()
        {
            await DoSetup(true, "How much is one plus two?", _standardKeywords);
            Assert.NotNull(_eventThatWasPublishedToTheBus);
            Assert.NotNull(_eventThatWasPublishedToTheBus.KeywordsDetected);
            Assert.Equal(2, _eventThatWasPublishedToTheBus.KeywordsDetected.Length);
            Assert.Equal(_standardKeywords[0], _eventThatWasPublishedToTheBus.KeywordsDetected[0]);
            Assert.Equal(_standardKeywords[1], _eventThatWasPublishedToTheBus.KeywordsDetected[1]);
        }

        [Fact]
        public async Task Id_OneMatch_IsCopiedFromConsumedEvent()
        {
            await DoSetup(true, "three is a match", _standardKeywords);
            Assert.Equal(_tweetMock.Object.Id, _eventThatWasPublishedToTheBus.Id);
        }

        [Fact]
        public async Task OrdinalCountNumber_OneMatch_IsCopiedFromConsumedEvent()
        {
            await DoSetup(true, "three is a match", _standardKeywords);
            Assert.Equal(_tweetMock.Object.OrdinalCountNumber, _eventThatWasPublishedToTheBus.OrdinalCountNumber);
        }

        [Fact]
        public async Task UnparsedData_OneMatch_IsCopiedFromConsumedEvent()
        {
            await DoSetup(true, "three is a match", _standardKeywords);
            Assert.Equal(_tweetMock.Object.UnparsedData, _eventThatWasPublishedToTheBus.UnparsedData);
        }

        [Fact]
        public async Task ReceivedDateTimeUtc_OneMatch_IsCopiedFromConsumedEvent()
        {
            await DoSetup(true, "three is a match", _standardKeywords);
            Assert.Equal(_tweetMock.Object.ReceivedDateTimeUtc, _eventThatWasPublishedToTheBus.ReceivedDateTimeUtc);
        }

        [Fact]
        public async Task GetKeywordsFromTweet_Standard_ObtainsActiveKeywordsFromPersistence()
        {
            // 
            // Since we may want to change keywords real-time and have this service respond immediately, 
            // we get the list of keywords from persistence every time we process a tweet.
            // The persistence implementation may decide to cache it.
            //
            await DoSetup(true, "three is a match", _standardKeywords);
            _persistenceMock.VerifyAll();
        }
    }
}
