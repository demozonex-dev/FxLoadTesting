using Fx.Injector;

using Fx.WebApi.Injector.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fx.WebApi.Injector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventGridInjectorController : ControllerBase
    {
        private ILogger<EventGridInjectorController> _logger;
        private IConfiguration _configuration;
        IInjector _injector;
        public EventGridInjectorController(ILogger<EventGridInjectorController> logger, 
                                           IConfiguration configuration,
                                           EventGridInjectorService service)
        {
            _logger = logger;        
            _configuration = configuration;
            _injector = service.Injector;

        }
        [HttpGet(Name = "inject")]
        public async Task<IActionResult> Inject()
        {
            _logger.LogInformation("Custo:InjectorController.Inject() called");
            string host = Fx.Helpers.NetworkInfo.GetHostName();
            Messages.DemoMessage data = new Messages.DemoMessage
            {
                Id = Guid.NewGuid().ToString(),
                Description = "message send",
                InjectorDate = DateTime.Now,
                Injector = _injector.InjectorType,
                Host = host,
                ClientIP= Request.Headers["CLIENT-IP"]                
            };
            string jsonData = JsonSerializer.Serialize(data);
            await _injector.SendAsync(jsonData);

            return Ok(data);
        }
      

    }
}
