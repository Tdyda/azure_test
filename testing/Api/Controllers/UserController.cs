using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testing.Application.Users.Create;

namespace testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest body, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateUserCommand(
                body.Email,
                body.Name,
                body.Surname,
                body.Username,
                body.Password), ct);
            
            return CreatedAtAction(nameof(GetById), new { Id = result.Id }, result);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
        {
            // w tym przykładzie nie implementujemy; zwróćmy 501 albo 404
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
