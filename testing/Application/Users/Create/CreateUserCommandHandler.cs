using MediatR;
using Microsoft.AspNetCore.Identity;
using testing.Application.Abstractions;
using testing.Application.Abstractions.Persistence;
using testing.Domain.ValueObjects;
using testing.Models;

namespace testing.Application.Users.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IPasswordHasher<User> _hasher;
    
    public CreateUserCommandHandler(IUserRepository users, IUnitOfWork uow, IClock clock, IPasswordHasher<User> hasher) 
    {
        _users = users;
        _uow = uow;
        _clock = clock;
        _hasher = hasher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        var email = Email.Create(cmd.Email);
        
        if(await _users.ExistsByEmail(email, ct))
            throw new InvalidOperationException("User with this email already exists.");

        var now = _clock.UtcNow;
        var user = User.Create(email, cmd.Username, cmd.Name, cmd.Surname, now);
        var hash = _hasher.HashPassword(user, cmd.Password);
        
        await _users.AddAsync(user, hash, ct);
        await _uow.SaveChangeAsync(ct);
        
        return new UserResponse(user.Id, user.Email.Value, user.Name, user.Surname, user.UserName, user.CreatedAtUtc);
    }
}