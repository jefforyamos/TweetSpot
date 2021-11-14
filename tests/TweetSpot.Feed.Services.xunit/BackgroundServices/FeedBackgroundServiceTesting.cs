using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TweetSpot.Models;
using TweetSpot.Net;
using TweetSpot.Resources;
using TweetSpot.ServiceBus.Commands;
using TweetSpot.ServiceBus.Events;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

#pragma warning disable CS8602 // Dereference of a possibly null reference.


namespace TweetSpot.BackgroundServices
{
    public class FeedBackgroundServiceTesting
    {
        private readonly ITestOutputHelper _output;

        public FeedBackgroundServiceTesting(ITestOutputHelper output)
        {
            _output = output;
            _service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
        }

        private Mock<ILogger<FeedBackgroundService>> _logger = new Mock<ILogger<FeedBackgroundService>>();
        private Mock<ITwitterFeedConfiguration> _config = new Mock<ITwitterFeedConfiguration>();
        private Mock<IBus> _bus = new Mock<IBus>();
        private Mock<IServiceProvider> _serviceProvider = new Mock<IServiceProvider>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Mock<IHttpClient> _httpClient = new Mock<IHttpClient>();
        private FeedBackgroundService _service;
        private ITwitterFeedConfiguration _defaultConfiguration = TwitterFeedConfiguration.Defaults;

        private void Init()
        {
            _logger = new Mock<ILogger<FeedBackgroundService>>();
            _config = new Mock<ITwitterFeedConfiguration>();
            _config.Setup(config => config.TwitterBearerToken).Returns("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            _config.Setup(config => config.StreamBufferSizeInKb).Returns(_defaultConfiguration.StreamBufferSizeInKb);
            _bus = new Mock<IBus>();
            _serviceProvider = new Mock<IServiceProvider>();
            _service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            _cancellationTokenSource = new CancellationTokenSource();
            _httpClient = new Mock<IHttpClient>();
            _serviceProvider.Setup(sp => sp.GetService(typeof(IHttpClient))).Returns(_httpClient.Object);
            var requestHeaders = new HttpClient().DefaultRequestHeaders; // ctor is private, have to borrow this collection from there
            _httpClient.Setup(c => c.DefaultRequestHeaders).Returns(requestHeaders);
        }


        [Fact]
        public async Task SafeExecuteAsync_Normal_RunsUntilCancelled()
        {
            const int maxCycles = 10;
            Init();
            int cycleCount = 0;
            var cancelToken = _cancellationTokenSource.Token;
            _bus.Setup(bus => bus.Publish<ITwitterFeedInitStarted>(It.IsAny<object>(), cancelToken))
                .Callback(() => _cancellationTokenSource.Cancel());
            var service = new Mock<FeedBackgroundService>(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            service.Setup(s => s.SafeExecuteAsync(cancelToken)).CallBase();
            service.Setup(s => s.InternalReadFromTwitterAsync(cancelToken)).Callback(() =>
            {
                if (++cycleCount >= maxCycles) _cancellationTokenSource.Cancel();
                _output.WriteLine($"Callback {cycleCount}");
            });
            await service.Object.SafeExecuteAsync(cancelToken);
            Assert.Equal(maxCycles, cycleCount);
        }

        [Fact]
        public async Task SafeExecuteAsync_ExceptionDuringCallToService_LogsTheException()
        {
            Init();
            var expectedException = new InvalidOperationException("Oh no, something went wrong");
            var cancelToken = _cancellationTokenSource.Token;
            _bus.Setup(bus => bus.Publish<ITwitterFeedInitStarted>(It.IsAny<object>(), cancelToken))
                .Callback(() => _cancellationTokenSource.Cancel());
            var service = new Mock<FeedBackgroundService>(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            service.Setup(s => s.SafeExecuteAsync(cancelToken)).CallBase();
            service.Setup(s => s.InternalReadFromTwitterAsync(cancelToken)).Callback(() =>
            {
                _cancellationTokenSource.Cancel();
                throw expectedException;
            });
            _logger.Setup(l => l.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                expectedException,
                It.IsAny<Func<object, Exception, string>>()));
            await service.Object.SafeExecuteAsync(cancelToken);
            _logger.Verify();
        }

        [Fact]
        public async Task FakeStream_FromFile_IsValid()
        {
            await using var stream = await GlobalTestResources.TweetStreamSmall1.OpenReadStreamAsync();
            var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var tweet = IncomingTweet.Create(line, DateTime.UtcNow, 0);
                Assert.NotNull(tweet);
            }
        }


        [Fact]
        public async Task InternalReadFromTwitterAsync_Normal_SendsStartupMessageToBusAnnouncingConfiguration()
        {
            Init();
            var cancelToken = _cancellationTokenSource.Token;
            _httpClient.Setup(c => c.GetStreamAsync(It.IsAny<Uri>(), cancelToken)).Returns(GlobalTestResources.TweetStreamSmall1.OpenReadStreamAsync());
            // _bus = new Mock<IBus>(MockBehavior.Strict);
            _bus.Setup(b => b.Publish<ITwitterFeedInitStarted>(It.IsAny<ITwitterFeedInitStarted>(), It.IsAny<CancellationToken>()));
            var service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            await service.InternalReadFromTwitterAsync(cancelToken);
            var startupMessageSentToBus = _bus.Invocations[0].Arguments[0] as ITwitterFeedInitStarted;
            Assert.Same(_config.Object, startupMessageSentToBus.Configuration);
            Assert.NotNull(startupMessageSentToBus?.Configuration?.TwitterBearerToken);
            Assert.Equal("ABC...XYZ", startupMessageSentToBus.Configuration.GetTwitterBearerTokenAbbreviation());
        }

        [Fact]
        public async Task InternalReadFromTwitterAsync_Normal_SendsCorrectNumberOfTweetsToTheBus()
        {
            Init();
            var cancelToken = _cancellationTokenSource.Token;
            _httpClient.Setup(c => c.GetStreamAsync(It.IsAny<Uri>(), cancelToken)).Returns(GlobalTestResources.TweetStreamSmall1.OpenReadStreamAsync());
            var service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            await service.InternalReadFromTwitterAsync(cancelToken);
            var tweetRecords = _bus.Invocations.Skip(1).Select(r => r.Arguments[0]).Cast<IProcessIncomingTweet>().ToArray();
            foreach (var tweetRecord in tweetRecords) _output.WriteLine(tweetRecord.UnparsedData);
            Assert.Equal(11, tweetRecords.Length);
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(5, 2)]
        [InlineData(3, 3)]
        [InlineData(2, 5)]
        [InlineData(1, 11)]

        public async Task TrafficReports_Normal_SendsCorrectNumberOfTrafficReportsToTheBus(int reportInterval, int expectedReports)
        {
            Init();
            var cancelToken = _cancellationTokenSource.Token;
            _httpClient.Setup(c => c.GetStreamAsync(It.IsAny<Uri>(), cancelToken)).Returns(GlobalTestResources.TweetStreamSmall1.OpenReadStreamAsync());
            _config.Setup(c => c.SpeedReportIntervalCount).Returns(reportInterval);
            var service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            await service.InternalReadFromTwitterAsync(cancelToken);
            var trafficRecords = _bus.Invocations.Select(r => r.Arguments[0]).OfType<IFeedSpeedReported>().ToArray();
            foreach (var trafficRec in trafficRecords) _output.WriteLine($"Current {trafficRec.CurrentSpeedTweetsPerSecond:N1}/sec, Average {trafficRec.AverageSpeedTweetsPerSecond:N1}/sec.");
            Assert.Equal(expectedReports, trafficRecords.Length);
        }

    }
}