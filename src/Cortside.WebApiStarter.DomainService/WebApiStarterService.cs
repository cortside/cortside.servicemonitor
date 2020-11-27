using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cortside.Common.DomainEvent;
using Cortside.WebApiStarter.Data;
using Cortside.WebApiStarter.Dto.Dto;
using Microsoft.Extensions.Logging;

namespace Cortside.WebApiStarter.DomainService {
    public class WebApiStarterService : IWebApiStarterService {
        private readonly IDatabaseContext db;
        private readonly IDomainEventPublisher publisher;
        private readonly ILogger<WebApiStarterService> logger;

        public WebApiStarterService(IDatabaseContext db,
            IDomainEventPublisher publisher,
            ILogger<WebApiStarterService> logger) {
            this.db = db;
            this.publisher = publisher;
            this.logger = logger;
        }

        public Task<WebApiStarterDto> CreateWebApiStarter(string parameter) {
            throw new NotImplementedException();
        }

        public Task<WebApiStarterDto> DeleteWebApiStarter(Guid WebApiStarterId) {
            throw new NotImplementedException();
        }

        public Task<WebApiStarterDto> GetWebApiStarter(Guid WebApiStarterId) {
            throw new NotImplementedException();
        }

        public Task<List<WebApiStarterDto>> GetWebApiStarters(List<Guid> WebApiStarterIds) {
            throw new NotImplementedException();
        }

        public Task<WebApiStarterDto> UpdateWebApiStarter(Guid WebApiStarterId, string parameter) {
            throw new NotImplementedException();
        }
    }
}
