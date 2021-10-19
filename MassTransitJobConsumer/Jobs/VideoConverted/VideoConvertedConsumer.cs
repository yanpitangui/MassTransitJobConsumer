using MassTransit;
using MassTransitJobConsumer.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitJobConsumer.Jobs
{
    public class VideoConvertedConsumer : IConsumer<VideoConverted>
    {
        private readonly ILogger<VideoConvertedConsumer> _logger;

        public VideoConvertedConsumer(ILogger<VideoConvertedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<VideoConverted> context)
        {
            _logger.LogInformation($"Video converted successfully from job id: {context.Message.JobId}");

            return Task.CompletedTask;
        }
    }
}
