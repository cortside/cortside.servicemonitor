using System.Linq;
using Cortside.HealthMonitor.WebApi.IntegrationTests.Helpers.HotDocsMock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests {
    public class TestWebApplicationFactory<Startup> : WebApplicationFactory<Startup> where Startup : class {
        protected override IHostBuilder CreateHostBuilder() {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: true)
                 .Build();

            var server = new SampleWireMock()
                .ConfigureBuilder();

            var section = configuration.GetSection("HealthCheckHostedService");
            section["Checks:1:Value"] = server.mockServer.Urls.First();
            section["Checks:2:Value"] = server.mockServer.Urls.First();

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => {
                    builder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                    .UseStartup<Startup>()
                    .UseSerilog();
                });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureServices(services => {
                // Build the service provider.
                var sp = services.BuildServiceProvider();
            });
        }
    }
}
