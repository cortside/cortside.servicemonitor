using System.Collections.Generic;
using System.Dynamic;
using Cortside.ServiceMonitor.Health;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cortside.ServiceMonitor.WebApi.Controllers {

    /// <summary>
    /// Represents the shared functionality/resources of the ServiceMonitor resource
    /// </summary>
    [ApiVersion("1")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/status")]
    //[Authorize]
    public class StatusController : Controller {
        private readonly ILogger logger;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the ServiceMonitorController
        /// </summary>
        public StatusController(ILogger<StatusController> logger, IMemoryCache cache) {
            this.logger = logger;
            this.cache = cache;
        }

        /// <summary>
        /// get status
        /// </summary>
        [HttpGet("")]
        //[Authorize(Constants.Authorization.Permissions.GetServiceMonitor)]
        //[ProducesResponseType(typeof(List<ServiceMonitorDto>), (int)HttpStatusCode.OK)]
        public IActionResult Get() {
            dynamic response = new ExpandoObject();
            var services = new Dictionary<string, CachedHealthModel>();

            foreach (var key in cache.GetKeys<string>()) {
                if (key.StartsWith("health::")) {
                    services.Add(key.Replace("health::", ""), cache.Get(key) as CachedHealthModel);
                }
            }

            response.Services = services;
            return Ok(response);
        }
    }
}
