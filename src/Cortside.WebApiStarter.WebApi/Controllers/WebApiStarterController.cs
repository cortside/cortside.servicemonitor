using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cortside.WebApiStarter.DomainService;
using Cortside.WebApiStarter.Dto.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cortside.WebApiStarter.WebApi.Controllers {

    /// <summary>
    /// Represents the shared functionality/resources of the WebApiStarter resource
    /// </summary>
    [ApiVersion("1")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/WebApiStarter")]
    [Authorize]
    public class WebApiStarterController : Controller {
        private readonly ILogger logger;
        private readonly IWebApiStarterService service;
        private const string GET_WebApiStarter_ROUTE = "GetWebApiStarter";

        /// <summary>
        /// Initializes a new instance of the WebApiStarterController
        /// </summary>
        public WebApiStarterController(ILogger<WebApiStarterController> logger, IWebApiStarterService service) {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetWebApiStarter)]
        [ProducesResponseType(typeof(List<WebApiStarterDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetWebApiStarter(List<Guid> ids) {
            var WebApiStartersList = await service.GetWebApiStarters(ids);
            return Ok(WebApiStartersList);
        }

        /// <summary>
        /// Gets a WebApiStarter by id
        /// </summary>
        /// <param name="id">the id of the WebApiStarter to get</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = GET_WebApiStarter_ROUTE)]
        [Authorize(Constants.Authorization.Permissions.GetWebApiStarter)]
        [ProducesResponseType(typeof(WebApiStarterDto), 200)]
        public async Task<IActionResult> GetWebApiStarter(Guid id) {
            var WebApiStarter = await service.GetWebApiStarter(id);
            return Ok(WebApiStarter);
        }

        /// <summary>
        /// Create a new WebApiStarter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateWebApiStarter)]
        [ProducesResponseType(typeof(WebApiStarterDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateWebApiStarter(string parameter) {
            var WebApiStarter = await service.CreateWebApiStarter(parameter);
            return Accepted(WebApiStarter);
        }
    }
}
