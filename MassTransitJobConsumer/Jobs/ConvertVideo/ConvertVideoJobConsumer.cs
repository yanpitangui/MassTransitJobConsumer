using MassTransit.JobService;
using MassTransitJobConsumer.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MassTransitJobConsumer.Jobs
{
    public class ConvertVideoJobConsumer :
        IJobConsumer<ConvertVideo>
    {
        readonly ILogger<ConvertVideoJobConsumer> _logger;

        public ConvertVideoJobConsumer(ILogger<ConvertVideoJobConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Run(JobContext<ConvertVideo> context)
        {
            _logger.LogInformation($"Received convert order to path: {context.Job.Path}");
            var rng = new Random();

            var variance = TimeSpan.FromMilliseconds(rng.Next(8399, 28377));

            await Task.Delay(variance);

            await context.Publish<VideoConverted>(new { 
                context.JobId         
                });
        }
    }
}
