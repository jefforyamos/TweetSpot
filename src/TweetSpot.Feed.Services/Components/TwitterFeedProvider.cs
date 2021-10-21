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

namespace TweetSpot.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        private readonly ILogger<TwitterFeedProvider> _logger;
        private readonly ITwitterFeedConfiguration _configuration;

        internal UtcNowFunc GetUtcNow = () => DateTime.UtcNow; // Allows for override during unit testing

        public TwitterFeedProvider(ILogger<TwitterFeedProvider> logger, ITwitterFeedConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async IAsyncEnumerable<IProcessIncomingTweet> ReadAsync([EnumeratorCancellation] CancellationToken cancelToken)
        {
            ulong ordinalCount = 0;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.TwitterBearerToken);
            using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelToken);
            using var streamReader = new System.IO.StreamReader(await streamTask);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (line == null) break;
                var tweet = ProcessIncomingTweet.Create(line, GetUtcNow(), ordinalCount++);
                if (tweet != null) yield return tweet;
                if (ordinalCount % 100 == 0)
                {
                    var elapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                    var countPerSecond = (double)ordinalCount / elapsedSeconds;
                    _logger.LogInformation($"{ordinalCount:N0} tweets received in {elapsedSeconds:N1} seconds = {countPerSecond:N1} per second.");
                }
            }
        }

    }
}