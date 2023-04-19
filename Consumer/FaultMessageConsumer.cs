using Common;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    internal class FaultMessageConsumer : IConsumer<Fault<Message>>
    {
        private readonly ILogger<FaultMessageConsumer> _logger;

        public FaultMessageConsumer(ILogger<FaultMessageConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Fault<Message>> context)
        {
            _logger.LogError("Fault: " + context.Message.Exceptions.FirstOrDefault().ToString());
            // update the dashboard
        }
    }
}
