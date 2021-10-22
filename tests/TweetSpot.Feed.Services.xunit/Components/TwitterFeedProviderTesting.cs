using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace TweetSpot.Components
{
    public class TwitterFeedProviderTesting
    {
        private readonly ITestOutputHelper _output;

        public TwitterFeedProviderTesting(ITestOutputHelper output)
        {
            _output = output;
        }


        //[Fact]
        //public async Task Run_CancelAfterFiveTweets_OnlyDeliversFive()
        //{
        // _output.WriteLine("Starting test");
        // var svc = new TwitterFeedProvider();
        // var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        // var list = new List<IProcessIncomingTweet>();
        // await foreach (var item in svc.ReadAsync(cancel.Token))
        // {
        //     list.Add(item);
        //     if( list.Count > 4 ) cancel.Cancel();
        // }
        // Assert.Equal(5, list.Count);
        //}

        [Fact]
        public void Run_InstancedWithServiceProvider_Success()
        {
            //var services = new ServiceCollection()
            //    .AddSingleton<ITwitterFeedProvider, TwitterFeedProvider>();
            //var svc = services.BuildServiceProvider()
            //    .GetRequiredService<ITwitterFeedProvider>();
            // Assert.NotNull(svc);
        }

        public interface IMyApp
        {
            string MyValue { get; }
        }

        public class MyApp : IMyApp
        {
            private readonly IConfiguration _config;

            public MyApp(IConfiguration config)
            {
                _config = config;
            }

            public string MyValue => _config.GetValue<string>("Testing.SomeValue");
        }


        [Fact]
        public void MessWithLoadingSecrets()
        {
            var builder = new ConfigurationBuilder();
            //builder.AddUserSecrets<TwitterFeedProviderTesting>();
            //// builder.AddUserSecrets<TwitterFeedProvider>();
            //builder.Build();
            //var secretValue = builder.Properties["A"];
            //Assert.Equal("Hello", secretValue);
        }


    }

    public class Temp
    {
        [Fact]
        public void Test()
        {
            Console.WriteLine("Hello");
        }
    }

}