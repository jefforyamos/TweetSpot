using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TweetSpot.Models;
using TweetSpot.ServiceBus.Commands;
using TweetSpot.Delegates;
using System.Diagnostics;
using MassTransit;
using MassTransit.Saga.Factories;
using TweetSpot.ServiceBus.Events;

namespace TweetSpot.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        private readonly ILogger<TwitterFeedProvider> _logger;
        private readonly ITwitterFeedConfiguration _configuration;
        private readonly IBus _bus;

        internal UtcNowFunc GetUtcNow = () => DateTime.UtcNow; // Allows for override during unit testing

        public TwitterFeedProvider(ILogger<TwitterFeedProvider> logger, ITwitterFeedConfiguration configuration, IBus bus)
        {
            _logger = logger;
            _configuration = configuration;
            _bus = bus;
        }

        internal string CreateBearerTokenAbbreviation()
        {
            const int MinimumLength = 10;
            var token = _configuration.TwitterBearerToken;
            var length = token?.Length ?? 0;
            if (length < MinimumLength) return "<BAD TOKEN>";
            var beginSegment = token?.Substring(0, 3) ?? string.Empty;
            var endSegment = token?.Substring(length - 3, 3) ?? string.Empty;
            return $"{beginSegment}...{endSegment}";
        }

        //public async IAsyncEnumerable<IProcessIncomingTweet> ReadAsync([EnumeratorCancellation] CancellationToken cancelToken)
        //{
        //    await _bus.Publish<ITwitterFeedInitStarted>(new {BearerTokenAbbreviation = CreateBearerTokenAbbreviation()}, cancelToken);
        //    ulong ordinalCount = 0;
        //    using var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.TwitterBearerToken);
        //    using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelToken);
        //    using var streamReader = new System.IO.StreamReader(await streamTask);
        //    var stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    ulong previousCycleCount = 0;
        //    double previousElapsedSeconds = 0.0;
        //    while (!streamReader.EndOfStream)
        //    {
        //        var line = await streamReader.ReadLineAsync();
        //        if (line == null) break;
        //        var tweet = ProcessIncomingTweet.Create(line, GetUtcNow(), ordinalCount++);
        //        if (tweet != null) yield return tweet;
        //        if (ordinalCount % 100 == 0)
        //        {
        //            var elapsedSeconds = stopWatch.Elapsed.TotalSeconds;
        //            var countPerSecond = (double) ordinalCount / elapsedSeconds;
        //            var currentSpeed = (double) (ordinalCount - previousCycleCount) / (elapsedSeconds - previousElapsedSeconds);
        //            _logger.LogInformation($"{ordinalCount:N0} tweets received in {elapsedSeconds:N1} seconds = {countPerSecond:N1}/sec.  Current speed = {currentSpeed:N1}/sec.");
        //            previousCycleCount = ordinalCount;
        //            previousElapsedSeconds = elapsedSeconds;
        //        }
        //    }
        //}

        public async Task ReadFromTwitterAsync(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    await InternalReadFromTwitterAsync(cancelToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception while reading feed from twitter");
                }
            }
        }

        internal async Task InternalReadFromTwitterAsync(CancellationToken cancelToken)
        {
            await _bus.Publish<ITwitterFeedInitStarted>(new {BearerTokenAbbreviation = CreateBearerTokenAbbreviation()}, cancelToken);
            ulong ordinalCount = 0;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.TwitterBearerToken);
            using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelToken);
            using var streamReader = new System.IO.StreamReader(await streamTask);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            FeedSpeedReported? previousReport = null;
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) break;
                var tweet = ProcessIncomingTweet.Create(line, GetUtcNow(), ordinalCount++);
                if (tweet != null)
                {
                    await _bus.Send<IProcessIncomingTweet>(tweet, cancelToken); // Triggers persistence
                }

                if (ordinalCount % (ulong) _configuration.SpeedReportIntervalCount == 0)
                {
                    // _logger.LogInformation($"{ordinalCount:N0} tweets received in {elapsedSeconds:N1} seconds = {countPerSecond:N1}/sec.  Current speed = {currentSpeed:N1}/sec.");
                    var currentReport = new FeedSpeedReported(stopWatch.Elapsed, ordinalCount, previousReport);
                    await _bus.Publish<IFeedSpeedReported>(currentReport, cancelToken);
                    previousReport = currentReport; // Save for next cycle
                }
            }
        }

    }



}