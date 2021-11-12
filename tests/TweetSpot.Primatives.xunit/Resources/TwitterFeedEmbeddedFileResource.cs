using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Resources
{
    /// <summary>
    /// Extends the <seealso cref="EmbeddedFileResource"/> by adding methods useful for reading Twitter feed data from an embedded file.
    /// </summary>
    public class TwitterFeedEmbeddedFileResource : EmbeddedFileResource
    {
        public TwitterFeedEmbeddedFileResource(Type classProvidingNamespaceAndAssembly, string relativePath)
            : base(classProvidingNamespaceAndAssembly, relativePath)
        {

        }

        public IEnumerable<IncomingTweet> GetIncomingTweets()
        {
            using var reader = GetReader();
            string? inputLine;
            ulong ordinalCount = 0;
            while ((inputLine = reader.ReadLine()) != null)
            {
                var tweet = IncomingTweet.Create(inputLine, DateTime.UtcNow, ordinalCount);
                if (tweet != null)
                {
                    yield return tweet;
                    ordinalCount++;
                }
            }
        }

        public async IAsyncEnumerable<IncomingTweet> GetIncomingTweetsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var reader = GetReader();
            string? inputLine;
            ulong ordinalCount = 0;
            while ((inputLine = await reader.ReadLineAsync()) != null)
            {
                var tweet = IncomingTweet.Create(inputLine, DateTime.UtcNow, ordinalCount);
                if (tweet != null)
                {
                    yield return tweet;
                    ordinalCount++;
                }
                if (cancellationToken.IsCancellationRequested) break;
            }
        }

    }
}
