using Cortside.Common.BootStrap;
using Cortside.WebApiStarter.Configuration;
using Cortside.WebApiStarter.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.WebApiStarter.BootStrap.Installer {
    public class ExampleHostedServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddSingleton(configuration.GetSection("ExampleHostedService").Get<ExampleHostedServiceConfiguration>());
            services.AddHostedService<ExampleHostedService>();
        }
    }
}
