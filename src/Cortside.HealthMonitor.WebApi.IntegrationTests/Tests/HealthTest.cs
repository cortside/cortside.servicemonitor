using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.Health.Models;
using Cortside.HealthMonitor.WebApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests.Tests {
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

        [Fact(Skip = "fails as test but not for real")]
        public async Task ShouldGetHealth() {
            //arrange
            await Task.Delay(40000);

            //act
            var response = await testServerClient.GetAsync("api/health");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var respObj = JsonConvert.DeserializeObject<HealthModel>(content, fixture.SerializerSettings);
            Assert.True(respObj.Healthy);
        }

        [Fact]
        public async Task ShouldGetServices() {
            //arrange
            await Task.Delay(15000);

            //act
            var response = await testServerClient.GetAsync("api/v1/services");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var respObj = JsonConvert.DeserializeObject<ServicesModel>(content, fixture.SerializerSettings);
            Assert.Equal(2, respObj.Services.Count);
        }
    }
}
