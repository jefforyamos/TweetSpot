using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TweetSpot.Delegates;
using TweetSpot.Models;
using TweetSpot.Net;
using TweetSpot.ServiceBus.Commands;
using TweetSpot.ServiceBus.Events;

namespace TweetSpot.BackgroundServices
{
    public class FeedBackgroundService : BackgroundService
    {
        private readonly ILogger<FeedBackgroundService> _logger;
        private readonly ITwitterFeedConfiguration _configuration;
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private ulong _ordinalCount;
        private FeedSpeedReported? _previousReport;

        internal UtcNowFunc GetUtcNow = () => DateTime.UtcNow; // Allows for override during unit testing

        /// <summary>
        /// Value to use for buffering size if the value is unspecified in the configuration.
        /// </summary>
        private const int DefaultBufferingSize = 1_024;

        public FeedBackgroundService(ILogger<FeedBackgroundService> logger, ITwitterFeedConfiguration configuration, IBus bus, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _bus = bus ?? throw new NullReferenceException(nameof(bus));
            _serviceProvider = serviceProvider ?? throw new NullReferenceException(nameof(serviceProvider));
        }

        /// <summary>
        /// Overridden to retry repeatedly until the service is stopped.
        /// </summary>
        /// <param name="stoppingToken">The token that tells us when to stop.</param>
        /// <returns>Creates a task that repeatedly attempts to gain connection and pull the feed</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await SafeExecuteAsync(stoppingToken);
        }

        internal virtual async Task SafeExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await InternalReadFromTwitterAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception while reading feed from twitter, restarting feed.");
                }
            }
        }


        /// <summary>
        /// Performs a single cycle read.  Resets the HTTP connection and everything.
        /// </summary>
        /// <param name="cancelToken">Tells us when to stop.</param>
        /// <returns>Returns a task that pulls tweets and publishes them to the bus.</returns>
        internal virtual async Task InternalReadFromTwitterAsync(CancellationToken cancelToken)
        {
            await _bus.Publish<ITwitterFeedInitStarted>(new { BearerTokenAbbreviation = _configuration.TwitterBearerTokenAbbreviation }, cancelToken);
            using var client = _serviceProvider.GetService<IHttpClient>() ?? throw new InvalidOperationException($"Bad dependency injection for {nameof(IHttpClient)}");
            client.Timeout = _configuration.ClientTimeout;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.TwitterBearerToken);
            using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelToken);
            await using var bufferedStream = new BufferedStream(await streamTask, _configuration.StreamBufferSize.GetValueOrDefault(DefaultBufferingSize));
            using var streamReader = new StreamReader(bufferedStream);
            _stopWatch.Start();
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) break;
                var tweet = ProcessIncomingTweet.Create(line, GetUtcNow(), _ordinalCount);
                if (tweet != null)
                {
                    await _bus.Publish<IProcessIncomingTweet>(tweet, cancelToken);
                    _ordinalCount++;
                }

                if ((_configuration.SpeedReportIntervalCount > 0) && (_ordinalCount % (ulong)_configuration.SpeedReportIntervalCount == 0))
                {
                    var currentReport = new FeedSpeedReported(_stopWatch.Elapsed, _ordinalCount, _previousReport);
                    await _bus.Publish<IFeedSpeedReported>(currentReport, cancelToken);
                    _previousReport = currentReport; // Save for next cycle
                }
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _configuration.DemandEssentialSettings();
            return base.StartAsync(cancellationToken);
        }
    }
}