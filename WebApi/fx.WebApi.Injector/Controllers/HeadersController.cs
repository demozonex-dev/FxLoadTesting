using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Injector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeadersController : ControllerBase
    {
       

        private readonly ILogger<HeadersController> _logger;

        public HeadersController(ILogger<HeadersController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Get")]
        public IActionResult Get()
        {
            _logger.LogInformation("Custo:HeadersController.Get() called");
          return Ok(Request.Headers);
        }
    }
}