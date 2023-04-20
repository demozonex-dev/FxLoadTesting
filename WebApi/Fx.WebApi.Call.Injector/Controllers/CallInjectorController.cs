using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Call.Injector.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CallInjectorController : ControllerBase
    {
        

        private readonly ILogger<CallInjectorController> _logger;
        HttpClient _httpClient;
        IConfiguration _configuration;
        public CallInjectorController(ILogger<CallInjectorController> logger,
                                  IConfiguration configuration,
                                  HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;


        }
        //[HttpGet(Name = "headers")]
        //public async Task<IActionResult> Headers()
        //{
        //    _logger.LogInformation("Custo: CallInjectorController.Get() called");
        //    string url = $"{_configuration["injectorUrl"]}/Headers";
        //    var response = await _httpClient.GetAsync(url);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return Ok(await response.Content.ReadAsStringAsync());
        //    }
        //    return BadRequest("KO");
        //}
        [HttpGet(Name = "call")]
        public async Task<IActionResult> Get([FromQuery] int delay)
        {
            _logger.LogInformation($"Custo : CallInjectorController.Get() call : Delay{delay}");
            string url = $"{_configuration["injectorUrl"]}/EventGridInjector";
            var response=await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            return BadRequest("KO");
        }
    }
}