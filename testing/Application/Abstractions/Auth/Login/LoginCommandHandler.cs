using MediatR;
using Microsoft.AspNetCore.Identity;
using testing.Application.Abstractions.Persistence;
using testing.Application.Abstractions.Security;
using testing.Models;

namespace testing.Application.Abstractions.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public LoginCommandHandler(IUserRepository users, IPasswordHasher<User> hasher, IJwtTokenGenerator jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(cmd.Email.ToLowerInvariant(), ct);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var result = _hasher.VerifyHashedPassword(user, user.Password, cmd.Password);
        if(result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid Credentials");
        
        var token = _jwt.GenerateToken(
            userId: user.Id,
            email: user.Email.Value,
            username: user.UserName,
            extraClaims: null);
        
        return new LoginResponse(token);
    }
}