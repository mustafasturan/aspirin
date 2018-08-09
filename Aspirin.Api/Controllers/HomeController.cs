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
    }
}
