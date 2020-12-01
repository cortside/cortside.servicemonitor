using Cortside.Common.BootStrap;
using Cortside.Health;
using Cortside.Health.Checks;
using Cortside.Health.Models;
using Cortside.Health.Recorders;
using Cortside.HealthMonitor.Health;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cortside.HealthMonitor.BootStrap.Installer {
    public class HealthInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            // health checks
            // telemetry recorder
            string instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            string endpointAddress = configuration["ApplicationInsights:EndpointAddress"];
            if (!string.IsNullOrEmpty(instrumentationKey) && !string.IsNullOrEmpty(endpointAddress)) {
                TelemetryConfiguration telemetryConfiguration = new TelemetryConfiguration(instrumentationKey, new InMemoryChannel { EndpointAddress = endpointAddress });
                TelemetryClient telemetryClient = new TelemetryClient(telemetryConfiguration);
                services.AddSingleton(telemetryClient);
                services.AddTransient<IAvailabilityRecorder, ApplicationInsightsRecorder>();
            } else {
                services.AddTransient<IAvailabilityRecorder, NullRecorder>();
            }

            // configuration
            services.AddSingleton(configuration.GetSection("HealthCheckHostedService").Get<HealthCheckServiceConfiguration>());
            services.AddSingleton(configuration.GetSection("Build").Get<BuildModel>());

            // checks
            services.AddTransient<UrlCheck>();
            services.AddTransient<DbContextCheck>();
            services.AddTransient<ExampleCheck>();
            services.AddTransient<CachingHealthCheck>();

            // check factory and hosted service
            services.AddSingleton(sp => {
                var cache = sp.GetService<IMemoryCache>();
                var logger = sp.GetService<ILogger<Check>>();
                var recorder = sp.GetService<IAvailabilityRecorder>();
                var configuration = sp.GetService<IConfiguration>();

                var factory = new CheckFactory(cache, logger, recorder, sp, configuration);
                factory.RegisterCheck("example", typeof(ExampleCheck));
                factory.RegisterCheck("caching", typeof(CachingHealthCheck));
                return factory as ICheckFactory;
            });
            services.AddHostedService<HealthCheckHostedService>();
        }
    }
}
