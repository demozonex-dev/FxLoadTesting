using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fx.WebApi.Call.Injector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadersController : ControllerBase
    {
        private readonly ILogger<HeadersController> _logger;
        public HeadersController(ILogger<HeadersController> logger)
        {
            _logger = logger;
        }
    }
}
