using Fx.WebApi.Call.Injector.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Call.Injector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadersController : ControllerBase
    {
        private readonly ILogger<HeadersController> _logger;
        IConfiguration _configuration;
        IHttpInjector _injector;
        public HeadersController(IConfiguration configuration,
                                 IHttpInjector injector,
                                 ILogger<HeadersController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _injector = injector;
        }
        [HttpGet()]
        [Route("injector")]
        public async Task<IActionResult> InjectorHeaders()
        {
            return Ok(await _injector.Headers());
        }
        [HttpGet()]        
        public IActionResult Headers()
        {
            return Ok(Request.Headers);
        }
    }
}
