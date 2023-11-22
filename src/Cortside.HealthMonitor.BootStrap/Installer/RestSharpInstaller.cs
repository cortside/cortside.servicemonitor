using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

namespace Cortside.HealthMonitor.BootStrap.Installer {
    public class RestSharpInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddSingleton(new RestClient());
        }
    }
}
