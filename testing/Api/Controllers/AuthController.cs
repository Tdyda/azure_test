using MediatR;
using Microsoft.AspNetCore.Mvc;
using testing.Application.Abstractions.Auth.Login;

namespace testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);
            return Ok(response);
        }
    }
}