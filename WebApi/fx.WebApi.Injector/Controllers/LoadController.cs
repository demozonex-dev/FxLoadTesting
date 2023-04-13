using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Injector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadController : ControllerBase
    {
        private readonly ILogger<LoadController> _logger;
        public LoadController(ILogger<LoadController> logger)
        {
            _logger = logger;
        }
        [HttpGet(Name = "error")]
        public IActionResult Error([FromQuery] int delay)
        {
            _logger.LogInformation($"Custo : LoadController.Error() call : Delay{delay}");
            Thread.Sleep( delay );

            return Ok($"Delay:{delay}");
        }
    }
    
}
