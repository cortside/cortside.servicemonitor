using Cortside.Common.BootStrap;
using Cortside.WebApiStarter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.WebApiStarter.BootStrap.Installer {
    public class DbContextInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddDbContext<DatabaseContext>(opt => {
                opt.UseSqlServer(configuration.GetSection("WebApiStarter").GetValue<string>("ConnectionString"));
                opt.EnableSensitiveDataLogging();
            });
            // TODO: this is creating another instance that is different than the one above and does not have EnableSensitiveDataLogging set on it -- need to fix
            services.AddTransient<IDatabaseContext, DatabaseContext>();

            // for DbContextCheck
            services.AddTransient<DbContext, DatabaseContext>();
        }
    }
}
