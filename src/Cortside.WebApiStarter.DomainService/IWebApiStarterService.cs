using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cortside.WebApiStarter.Dto.Dto;

namespace Cortside.WebApiStarter.DomainService {
    public interface IWebApiStarterService {
        Task<WebApiStarterDto> CreateWebApiStarter(string parameter);
        Task<WebApiStarterDto> GetWebApiStarter(Guid WebApiStarterId);
        Task<List<WebApiStarterDto>> GetWebApiStarters(List<Guid> WebApiStarterIds);
        Task<WebApiStarterDto> UpdateWebApiStarter(Guid WebApiStarterId, string parameter);
        Task<WebApiStarterDto> DeleteWebApiStarter(Guid WebApiStarterId);
    }
}
