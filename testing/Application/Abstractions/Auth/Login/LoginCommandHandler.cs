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
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshToken;
    private readonly IUnitOfWork _uow;

    public LoginCommandHandler(IUserRepository users, IPasswordHasher<User> hasher, ITokenService tokenService, IRefreshTokenRepository refreshToken, IUnitOfWork uow)
    {
        _users = users;
        _hasher = hasher;
        _tokenService = tokenService;
        _refreshToken = refreshToken;
        _uow = uow;
    }

    public async Task<LoginResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(cmd.Email.ToLowerInvariant(), ct);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var result = _hasher.VerifyHashedPassword(user, user.Password, cmd.Password);
        if(result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid Credentials");
        
        var (access, refresh) = _tokenService.IssueTokenPair(user.Id, user.Email.Value, user.UserName);
        
        var now = DateTime.UtcNow;
        var tokenHash = _tokenService.HashRefreshToken(refresh);
        var rt = RefreshToken.Create(user.Id, tokenHash, now, _tokenService.GetRefreshExpiryUtc(), cmd.Ip, cmd.UserAgent);
        await _refreshToken.AddAsync(rt, ct);
        await _uow.SaveChangeAsync(ct);
        
        return new LoginResponse(access, refresh);
    }
}