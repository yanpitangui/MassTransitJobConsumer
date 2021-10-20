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
            try
            {
                var variance = TimeSpan.FromMilliseconds(rng.Next(100, 3000));

                await Task.Delay(variance);
                if (rng.Next(1, 3) == 1) throw new ApplicationException();

                await context.Publish<VideoConverted>(new
                {
                    context.JobId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while converting video");
                throw;
            }

        }
    }
}
