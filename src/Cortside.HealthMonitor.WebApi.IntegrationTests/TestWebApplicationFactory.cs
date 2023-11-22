using Cortside.HealthMonitor.WebApi.IntegrationTests.Helpers.HotDocsMock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests {
    public class TestWebApplicationFactory<Startup> : WebApplicationFactory<Startup> where Startup : class {
        protected override IHostBuilder CreateHostBuilder() {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: false)
                 .AddJsonFile("build.json", optional: false, reloadOnChange: false)
                 .Build();

            var server = new SampleWireMock()
                .ConfigureBuilder();

            var section = configuration.GetSection("HealthCheckHostedService");
            section["Checks:1:Value"] = server.mockServer.Url;
            section["Checks:2:Value"] = server.mockServer.Url;

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => {
                    builder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                        .UseStartup<Startup>()
                        .ConfigureTestServices(sc => {
                            ResolveSerializerSettings(sc);
                        });
                })
                .UseSerilog();
        }

        private void ResolveSerializerSettings(IServiceCollection services) {
            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var o = scopedServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            SerializerSettings = o.Value.SerializerSettings;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureServices(services => {
                // Build the service provider.
                var sp = services.BuildServiceProvider();
            });
        }

        public JsonSerializerSettings SerializerSettings { get; private set; }
    }
}
