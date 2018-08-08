using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aspirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HomeController>();
        }
        [HttpGet]
        public IActionResult Ping()
        {
            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;

            _logger.LogInformation("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
            return Ok("Pong");
        }
    }
}
