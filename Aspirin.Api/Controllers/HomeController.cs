using System.Threading.Tasks;
using Aspirin.Api.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aspirin.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok(await _mediator.Send(new Ping()));
        }

        [HttpGet, Route("write/{key}")]
        public async Task<IActionResult> WriteHelloToRedis([FromRoute]string key)
        {
            await _mediator.Send(new WriteHelloToRedis(key));
            return Ok();
        }

        [HttpGet, Route("read/{key}")]
        public async Task<IActionResult> ReadFromRedis([FromRoute]string key)
        {
            return Ok(await _mediator.Send(new ReadFromRedis(key)));
        }
    }
}
