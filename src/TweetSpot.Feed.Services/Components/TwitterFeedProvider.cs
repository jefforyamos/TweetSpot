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

namespace TweetSpot.Components
{
    public class TwitterFeedProvider : ITwitterFeedProvider
    {
        private readonly ILogger<TwitterFeedProvider> _logger;
        private readonly ITwitterFeedConfiguration _configuration;

     

        public TwitterFeedProvider(ILogger<TwitterFeedProvider> logger, ITwitterFeedConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private async Task<IEnumerable<object>> GetTwitterFeed(CancellationToken cancelToken)
        {
            var cancelInFewSeconds = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear(); // Don't accept any responses except....
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")); // Except this mime type
            // client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter"); // While claiming to be this caller
            if( _configuration.TwitterBearerToken != null )
                client.DefaultRequestHeaders.Add($"Authorization", "Bearer {_configuration.TwitterBearerToken}");
            //var serializerOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelInFewSeconds.Token);
            var repositories = await JsonSerializer.DeserializeAsync<List<object>>(await streamTask, null, cancelInFewSeconds.Token); 
            return repositories ?? new List<object>();
        }

        public async IAsyncEnumerable<IProcessIncomingTweet> ReadAsync([EnumeratorCancellation] CancellationToken cancelToken)
        {
            //var data = await GetTwitterFeed(cancelToken);
            //foreach (var item in data)
            //{
            //    yield return new FakeTweet();
            //}
            var cancelInFewSeconds = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear(); // Don't accept any responses except....
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")); // Except this mime type
            // client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter"); // While claiming to be this caller
            if (_configuration.TwitterBearerToken != null)
                client.DefaultRequestHeaders.Add($"Authorization", "Bearer {_configuration.TwitterBearerToken}");
            var rawData = await client.GetStringAsync(_configuration.SampledStreamUri, cancelInFewSeconds.Token);
            yield break;
            //var serializerOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            //using var streamTask = client.GetStreamAsync(_configuration.SampledStreamUri, cancelInFewSeconds.Token);
            //var myData = streamTask.Result.re
            //var objectStream = await JsonSerializer.DeserializeAsync<IEnumerable<object>>(await streamTask, null, cancelInFewSeconds.Token);
            //if (objectStream == null)
            //{
            //    yield break;
            //}
            //else
            //{
            //    foreach (var item in objectStream)
            //    {
            //        yield return new FakeTweet();
            //    }
            //}
        }

        private class FakeTweet : ServiceBus.Commands.IProcessIncomingTweet
        {
            public int id { get; set; }

        }

    }
}