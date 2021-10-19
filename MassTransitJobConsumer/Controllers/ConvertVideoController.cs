using MassTransit;
using MassTransit.Contracts.JobService;
using MassTransitJobConsumer.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitJobConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertVideoController : ControllerBase
    {

        readonly IRequestClient<ConvertVideo> _client;

        private readonly ILogger<ConvertVideoController> _logger;

        public ConvertVideoController(ILogger<ConvertVideoController> logger, IRequestClient<ConvertVideo> client)
        {
            _logger = logger;
            _client = client;
        }


        [HttpPost("{path}")]
        public async Task<IActionResult> Post(string path)
        {
            _logger.LogInformation("Sending job: {Path}", path);
            Response<JobSubmissionAccepted> response = await _client.GetResponse<JobSubmissionAccepted>(new
            {
                path
            });

            return Ok(new
            {
                response.Message.JobId,
                path
            });
        }
    }
}
