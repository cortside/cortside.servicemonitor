using Cortside.AspNetCore;
using Cortside.AspNetCore.AccessControl;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.Swagger;
using Cortside.Health;
using Cortside.HealthMonitor.BootStrap;
using Cortside.HealthMonitor.Health;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RestSharp;

namespace Cortside.HealthMonitor.WebApi {
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup : IWebApiStartup {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        [ActivatorUtilitiesConstructor]
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// Startup
        /// </summary>
        public Startup() {
        }

        /// <summary>
        /// Config
        /// </summary>
        private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Set configuration after creation
        /// </summary>
        /// <param name="config"></param>
        public void UseConfiguration(IConfiguration config) {
            Configuration = config;
        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // setup default json serializer settings
            JsonConvert.DefaultSettings = JsonNetUtility.GlobalDefaultSettings;

            // add ApplicationInsights telemetry
            var serviceName = Configuration["Service:Name"];
            var config = Configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsServiceOptions>();
            services.AddApplicationInsights(serviceName, config);

            // add health services
            services.AddHealth(o => {
                o.UseConfiguration(Configuration);
                o.AddCustomCheck("caching", typeof(CachingHealthCheck));
            });

            // add controllers and all of the api defaults
            services.AddApiDefaults();

            // Add the access control using IdentityServer and PolicyServer
            services.AddAccessControl(Configuration);

            // Add swagger with versioning and OpenID Connect configuration using Newtonsoft
            services.AddSwagger(Configuration, "HealthMonitor Api", "HealthMonitor Api", new[] { "v1" });

            // setup and register boostrapper and it's installers
            // this should be last
            services.AddBootStrapper<DefaultApplicationBootStrapper>(Configuration, _ => {
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            app.UseApiDefaults(Configuration);
            app.UseSwagger("HealthMonitor Api", provider);

            // order of the following matters
            app.UseAuthentication();
            //app.UseSubjectPrincipal(); // intentionally set after UseAuthentication
            app.UseRouting();
            //app.UseAuthorization(); // intentionally set after UseRouting
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
