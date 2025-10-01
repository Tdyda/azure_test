using MediatR;
using testing.Application.Abstractions.Persistence;
using testing.Models;

namespace testing.Application.Abstractions.Auth.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IClock _clock;
    private readonly IUnitOfWork _uow;

    public RefreshTokenCommandHandler(IRefreshTokenRepository repo, IUserRepository users, ITokenService tokens,
        IClock clock, IUnitOfWork uow)
    {
        _repo = repo;
        _users = users;
        _tokens = tokens;
        _clock = clock;
        _uow = uow;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var hash = _tokens.HashRefreshToken(cmd.RefreshToken);
        var existing = await _repo.GetByHashAsync(hash, ct);
        if (existing is null) throw new UnauthorizedAccessException("Invalid refresh token");

        var now = _clock.UtcNow;

        if (!existing.IsActive(now))
        {
            var chain = await _repo.GetAllByUserAsync(existing.UserId, ct);
            foreach (var t in chain.Where(t => t.RevokedAtUtc is null))
            {
                t.Revoke(now);
            }

            await _uow.SaveChangeAsync(ct);
            throw new UnauthorizedAccessException("Refresh token revoked or expired");
        }
        
        var user = await _users.GetByIdAsync(existing.UserId, ct) ?? throw new UnauthorizedAccessException("User not found");
        var (newAccess, newRefresh) = _tokens.IssueTokenPair(user.Id, user.Email.Value, user.UserName);
        
        var newHash = _tokens.HashRefreshToken(newRefresh);
        var newToken = RefreshToken.Create(user.Id, newHash, now, _tokens.GetRefreshExpiryUtc(), cmd.Ip, cmd.UserAgent);

        await _repo.AddAsync(newToken, ct);
        existing.ReplaceWith(newToken.Id, now);
        await _uow.SaveChangeAsync(ct);
        
        return new RefreshTokenResponse(newAccess, newRefresh);
    }
}