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

        private void Init()
        {
            _logger = new Mock<ILogger<FeedBackgroundService>>();
            _config = new Mock<ITwitterFeedConfiguration>();
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

        //private Task<Stream> GetFakeStreamOfData()
        //{
        //    var enc = Encoding.UTF8;
        //    var buffer = enc.GetBytes(Properties.Resources.TweetStreamSmall_1);
        //    var memoryStream = new MemoryStream(buffer);
        //    return Task.FromResult(memoryStream as Stream);
        //}

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
        public async Task InternalReadFromTwitterAsync_Normal_NoErrors()
        {
            Init();
            var cancelToken = _cancellationTokenSource.Token;
            _httpClient.Setup(c => c.GetStreamAsync(It.IsAny<Uri>(), cancelToken)).Returns(GlobalTestResources.TweetStreamSmall1.OpenReadStreamAsync());
            _bus = new Mock<IBus>(MockBehavior.Strict);
            _bus.Setup(b => b.Publish<ITwitterFeedInitStarted>(It.IsAny<object>(), cancelToken));
            //for (var x = 0; x < 5; x++)
            //{
            //    _bus.Setup(b => b.Publish<IProcessIncomingTweet>(It.IsAny<IProcessIncomingTweet>(), cancelToken))
            //        .Callback(() =>
            //            {
            //                _output.WriteLine("Tweet");
            //            }
            //            );
            //}

            var service = new FeedBackgroundService(_logger.Object, _config.Object, _bus.Object, _serviceProvider.Object);
            await service.InternalReadFromTwitterAsync(cancelToken);
            _bus.Verify();
        }
    }
}