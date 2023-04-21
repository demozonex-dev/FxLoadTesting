using Fx.WebApi.Call.Injector.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Call.Injector.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CallInjectorController : ControllerBase
    {
        

        private readonly ILogger<CallInjectorController> _logger;
        IHttpInjector _injector;
        IConfiguration _configuration;
        public CallInjectorController(ILogger<CallInjectorController> logger,
                                        IConfiguration configuration,
                                        IHttpInjector injector)
                                  
        {
            _logger = logger;
            _injector = injector;
            _configuration = configuration;


        }
        
        [HttpGet(Name = "call")]
        public async Task<IActionResult> Get([FromQuery] int delay)
        {
            _logger.LogInformation($"Custo : CallInjectorController.Get() call : Delay{delay}");

            var response = await _injector.EvengridInjector();
            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            return BadRequest("KO");
        }
    }
}