using System.IO;
using Cortside.WebApiStarter.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cortside.WebApiStarter.WebApi.Data {

    /// <summary>
    /// Design time context factory for EF
    /// https://codingblast.com/entityframework-core-idesigntimedbcontextfactory/
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext> {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        public DesignTimeDbContextFactory() {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public DesignTimeDbContextFactory(IHttpContextAccessor httpContextAccessor) {
            this.httpContextAccessor = httpContextAccessor;

        }

        /// <summary>
        /// Create context
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DatabaseContext CreateDbContext(string[] args) {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DatabaseContext>();

            var connectionString = configuration.GetSection("WebApiStarter").GetValue<string>("ConnectionString");

            builder.UseSqlServer(connectionString);

            return new DatabaseContext(builder.Options, httpContextAccessor);
        }
    }
}
