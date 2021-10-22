using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TweetSpot.AllInOne.ServiceBus.Consumers;
using TweetSpot.BackgroundServices;
using TweetSpot.Components;
using TweetSpot.Models;

namespace TweetSpot.AllInOne
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
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<SampleMessageConsumer>();
                        x.AddConsumer<LogEventsToConsoleConsumer>(); // To show us what's going on

                        x.UsingInMemory((context, cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                    services.AddMassTransitHostedService(true);

                   // services.AddHostedService<SampleBackgroundWorker>();
                    services.AddSingleton<ITwitterFeedConfiguration, TwitterFeedConfiguration>();
                    services.AddTransient<ITwitterFeedProvider, TwitterFeedProvider>();
                    services.AddHostedService<FeedBackgroundService>();

                });
    }

   
}
