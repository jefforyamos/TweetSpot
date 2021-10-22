using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TweetSpot.AllInOne
{
    public class SampleBackgroundWorker : BackgroundService
    {
        private readonly ILogger<SampleBackgroundWorker> _logger;
        readonly IBus _bus;

        public SampleBackgroundWorker(ILogger<SampleBackgroundWorker> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bus.Publish(new SampleMessage { Text = $"The time is {DateTimeOffset.Now}" });

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class SampleMessage
    {
        public string? Text { get; set; }
    }

    public class SampleMessageConsumer :
        IConsumer<SampleMessage>
    {
        readonly ILogger<SampleMessageConsumer> _logger;

        public SampleMessageConsumer(ILogger<SampleMessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SampleMessage> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Text);

            return Task.CompletedTask;
        }
    }

}