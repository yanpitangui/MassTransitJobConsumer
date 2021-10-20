using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.JobService;
using MassTransitJobConsumer.Contracts;
using System;

namespace MassTransitJobConsumer.Jobs
{
    public class ConvertVideoJobConsumerDefinition :
        ConsumerDefinition<ConvertVideoJobConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ConvertVideoJobConsumer> consumerConfigurator)
        {
            consumerConfigurator.Options<JobOptions<ConvertVideo>>(options =>
                options.SetJobTimeout(TimeSpan.FromMinutes(10)).SetConcurrentJobLimit(10));
        }
    }
}
