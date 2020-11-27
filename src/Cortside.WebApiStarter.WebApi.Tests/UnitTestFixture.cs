using System;
using System.Collections.Generic;
using Cortside.WebApiStarter.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Cortside.WebApiStarter.WebApi.Tests {
    public class UnitTestFixture {
        private readonly List<Mock> mocks;
        protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        public UnitTestFixture() {
            mocks = new List<Mock>();
        }

        public Mock<T> Mock<T>() where T : class {
            var mock = new Mock<T>();
            this.mocks.Add(mock);
            return mock;
        }

        public IDatabaseContext GetDatabaseContext() {
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase($"db-{Guid.NewGuid():d}").Options;
            var databaseContextStub = new DatabaseContext(databaseContextOptions, httpContextAccessorMock.Object);
            return databaseContextStub;
        }

        public void TearDown() {
            this.mocks.ForEach(m => m.VerifyAll());
        }
    }
}
