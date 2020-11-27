using Cortside.Common.DomainEvent;
using Cortside.WebApiStarter.Data;
using Moq;
using Xunit.Abstractions;

namespace Cortside.WebApiStarter.DomainService.Tests {
    public class WebApiStartererviceTest : DomainServiceTest<IWebApiStarterService> {

        private readonly IDatabaseContext databaseContext;
        private readonly Mock<IDomainEventPublisher> domainEventPublisherMock;
        private readonly ITestOutputHelper testOutputHelper;

        public WebApiStartererviceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
            domainEventPublisherMock = testFixture.Mock<IDomainEventPublisher>();
            this.testOutputHelper = testOutputHelper;
        }

    }
}
