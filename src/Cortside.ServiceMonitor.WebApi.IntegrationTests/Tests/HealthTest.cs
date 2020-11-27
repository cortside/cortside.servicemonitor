using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cortside.Health.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.ServiceMonitor.WebApi.IntegrationTests.Tests {
    public class HealthTest : IClassFixture<TestWebApplicationFactory<Startup>> {
        private readonly TestWebApplicationFactory<Startup> fixture;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly HttpClient testServerClient;

        public HealthTest(TestWebApplicationFactory<Startup> fixture, ITestOutputHelper testOutputHelper) {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
            testServerClient = fixture.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Test() {
            //arrange

            //act
            var response = await testServerClient.GetAsync("api/health");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var respObj = JsonConvert.DeserializeObject<HealthModel>(response.Content.ReadAsStringAsync().Result);
            Assert.True(respObj.Healthy);
        }
    }
}
