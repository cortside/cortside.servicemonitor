using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Cortside.Common.Hosting;
using Cortside.WebApiStarter.Configuration;
using Microsoft.Extensions.Logging;

namespace Cortside.WebApiStarter.Hosting {
    public class ExampleHostedService : TimedHostedService {

        public ExampleHostedService(ILogger<ExampleHostedService> logger, ExampleHostedServiceConfiguration config) : base(logger, config.Enabled, config.Interval, true) {
        }

        protected override async Task ExecuteIntervalAsync() {
            var correlationId = CorrelationContext.GetCorrelationId();
            logger.LogInformation($"CorrelationId: {correlationId}");
        }
    }
}
