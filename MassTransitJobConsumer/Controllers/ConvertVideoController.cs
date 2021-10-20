using System;
using MassTransit;
using MassTransit.Contracts.JobService;
using MassTransitJobConsumer.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration.JobService;
using MassTransit.JobService.Components.StateMachines;
using Microsoft.EntityFrameworkCore;

namespace MassTransitJobConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertVideoController : ControllerBase
    {

        private readonly IRequestClient<ConvertVideo> _client;

        private readonly ILogger<ConvertVideoController> _logger;

        private readonly JobServiceSagaDbContext _context;

        public ConvertVideoController(ILogger<ConvertVideoController> logger, IRequestClient<ConvertVideo> client, JobServiceSagaDbContext context)
        {
            _logger = logger;
            _client = client;
            _context = context;
        }


        [HttpPost("{path}")]
        public async Task<IActionResult> Post(string path)
        {
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var job = await _context.Set<JobSaga>().FirstOrDefaultAsync(x => x.CorrelationId == id);
            return job == null ? NotFound() : Ok(job);
        }
    }
}
