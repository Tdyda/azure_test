using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testing.Application.Abstractions.Auth.Login;
using testing.Application.Abstractions.Auth.Logout;
using testing.Application.Abstractions.Auth.Refresh;

namespace testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();
            var res = await _mediator.Send(new LoginCommand(request.Email, request.Password, ip, ua), ct);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenRequest req,
            CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();
            var res = await _mediator.Send(new RefreshTokenCommand(req.RefreshToken, ip, ua), ct);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenRequest req, CancellationToken ct)
        {
            await _mediator.Send(new RevokeRefreshTokenCommand(req.RefreshToken), ct);
            return NoContent();
        }
    }
}