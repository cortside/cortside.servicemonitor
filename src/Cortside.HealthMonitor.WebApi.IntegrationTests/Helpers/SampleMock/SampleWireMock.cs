using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests.Helpers.HotDocsMock {
    public class SampleWireMock {
        public WireMockServer mockServer;
        public SampleWireMock() {
            if (mockServer == null) {
                mockServer = WireMockServer.Start();
            }
        }
        public SampleWireMock ConfigureBuilder() {
            mockServer
                .Given(
                    Request.Create().WithPath("/".Split('?')[0]).UsingGet()
                    )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                    );

            mockServer
                .Given(
                    Request.Create().WithPath("/").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );

            mockServer
                .Given(
                    Request.Create().WithPath("/api/health").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );

            return this;
        }
    }
}
