using System;
using System.Security.Claims;
using Cortside.WebApiStarter.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Cortside.WebApiStarter.DomainService.Tests {
    public abstract class DomainServiceTest<T> : IDisposable {

        protected T service;
        protected UnitTestFixture testFixture;
        protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        protected DomainServiceTest() {
            testFixture = new UnitTestFixture();

        }

        protected IDatabaseContext GetDatabaseContext() {
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase($"db-{Guid.NewGuid():d}").Options;
            var databaseContextStub = new DatabaseContext(databaseContextOptions, httpContextAccessorMock.Object);
            return databaseContextStub;
        }

        public void SetupHttpUser(Claim claim) {
            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            Mock<ClaimsPrincipal> user = new Mock<ClaimsPrincipal>();
            if (claim != null) {
                httpContext.SetupGet(x => x.User).Returns(user.Object);
                this.httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
                user.Setup(x => x.FindFirst(claim.Type)).Returns(claim);
            }
        }

        public void Dispose() {
            testFixture.TearDown();
        }
    }
}
