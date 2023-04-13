using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Call.Injector.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CallInjectorController : ControllerBase
    {
        

        private readonly ILogger<CallInjectorController> _logger;
        static HttpClient _httpClient;
        IConfiguration _configuration;
        public CallInjectorController(ILogger<CallInjectorController> logger,
                                  IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _configuration = configuration;

        }

        [HttpGet(Name = "call")]
        public async Task<IActionResult> Get([FromQuery] int delay)
        {
            _logger.LogInformation($"Custo : CallInjectorController.Get() call : Delay{delay}");
            string url = _configuration["injectorUrl"] + delay;
            var response=await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            return BadRequest("KO");
        }
    }
}