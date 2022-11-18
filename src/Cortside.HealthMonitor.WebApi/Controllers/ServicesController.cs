using System.Collections.Generic;
using System.Net;
using Cortside.Health;
using Cortside.Health.Models;
using Cortside.HealthMonitor.Health;
using Cortside.HealthMonitor.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cortside.HealthMonitor.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the HealthMonitor resource
    /// </summary>
    [ApiVersion("1")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/services")]
    //[Authorize]
    public class ServicesController : Controller {
        private readonly ILogger logger;
        private readonly IMemoryCache cache;
        private readonly HealthCheckServiceConfiguration config;

        /// <summary>
        /// Initializes a new instance of the HealthMonitorController
        /// </summary>
        public ServicesController(ILogger<ServicesController> logger, IMemoryCache cache, HealthCheckServiceConfiguration config) {
            this.logger = logger;
            this.cache = cache;
            this.config = config;
        }

        /// <summary>
        /// get status for all services
        /// </summary>
        [HttpGet("")]
        [ProducesResponseType(typeof(ServicesModel), (int)HttpStatusCode.OK)]
        public IActionResult Get() {
            dynamic response = new ServicesModel();
            var services = new Dictionary<string, CachedHealthModel>();

            foreach (var key in cache.GetKeys<string>()) {
                if (key.StartsWith("health::")) {
                    services.Add(key.Replace("health::", ""), cache.Get(key) as CachedHealthModel);
                }
            }

            response.Services = services;
            return Ok(response);
        }

        /// <summary>
        /// get status for all services
        /// </summary>
        [HttpGet("{service}")]
        [ProducesResponseType(typeof(HealthModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Get(string service) {
            if (service == config.Name) {
                HealthModel result = cache.Get(config.Name) as HealthModel;
                if (result == null) {
                    return NotFound();
                }
                return Ok(result);
            } else {
                var result = cache.Get("health::" + service) as CachedHealthModel;

                if (result != null) {
                    if (result.Data != null) {
                        return Ok(result.Data);
                    } else {
                        return Ok(result);
                    }
                } else {
                    return NotFound();
                }
            }
        }
    }
}
