using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests.Tests {
    public class HealthMonitorTests : IClassFixture<TestWebApplicationFactory<Startup>> {
        private readonly TestWebApplicationFactory<Startup> fixture;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly HttpClient testServerClient;

        public HealthMonitorTests(TestWebApplicationFactory<Startup> fixture, ITestOutputHelper testOutputHelper) {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
            testServerClient = fixture.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });
        }
    }
}
