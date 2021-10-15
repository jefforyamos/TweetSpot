using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JeffTwit.ServiceBus.Commands;
using JeffTwit.ServiceBus.Events;
using Xunit;

namespace JeffTwit.Components
{
    public class TwitterFeedProviderTesting
    {
        [Fact]
        public async Task Run_CancelAfterFiveTweets_OnlyDeliversFive()
        {
            var svc = new TwitterFeedProvider();
            var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var list = new List<IProcessIncomingTweet>();
            await foreach (var item in svc.ReadAsync(cancel.Token))
            {
                list.Add(item);
                if( list.Count > 4 ) cancel.Cancel();
            }
        }
    }
}