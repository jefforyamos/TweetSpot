using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TweetSpot.BackgroundServices;
using TweetSpot.Components;
using TweetSpot.Models;

namespace TweetSpot.Feed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<TwitterFeedProvider>();
                    }
                    else
                    {
                        builder.AddEnvironmentVariables();
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ITwitterFeedConfiguration, TwitterFeedConfiguration>();
                    services.AddTransient<ITwitterFeedProvider, TwitterFeedProvider>();
                    services.AddHostedService<FeedBackgroundService>();
                });
    }
}
