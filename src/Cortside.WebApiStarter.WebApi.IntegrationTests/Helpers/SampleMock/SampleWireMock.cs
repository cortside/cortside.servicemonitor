using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Cortside.WebApiStarter.WebApi.IntegrationTests.Helpers.HotDocsMock {
    public class SampleWireMock {

        public FluentMockServer mockServer;
        public SampleWireMock() {
            if (mockServer == null) {
                mockServer = FluentMockServer.Start();
            }
        }
        public SampleWireMock ConfigureBuilder() {

            mockServer
                .Given(
                    Request.Create().WithPath($"/".Split('?')[0]).UsingGet()
                    )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                    );

            mockServer
                .Given(
                    Request.Create().WithPath($"/").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );

            mockServer
                .Given(
                    Request.Create().WithPath($"/api/health").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );

            return this;
        }
    }
}
