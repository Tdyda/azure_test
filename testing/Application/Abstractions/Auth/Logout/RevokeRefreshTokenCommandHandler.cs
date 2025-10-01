using MediatR;
using testing.Application.Abstractions.Persistence;

namespace testing.Application.Abstractions.Auth.Logout;

public sealed class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly ITokenService _tokens;
    private IClock _clock;
    private IUnitOfWork _uow;
    
    public RevokeRefreshTokenCommandHandler(IRefreshTokenRepository repo, ITokenService tokens, IClock clock, IUnitOfWork uow)
    {
        _repo = repo;
        _tokens = tokens;
        _clock = clock;
        _uow = uow;
    }
    
    public async Task<Unit> Handle(RevokeRefreshTokenCommand cmd, CancellationToken ct)
    {
        var hash = _tokens.HashRefreshToken(cmd.RefreshToken);
        var token = await _repo.GetByHashAsync(hash, ct);
        if (token.RevokedAtUtc is null)
        {
            token.Revoke(_clock.UtcNow);
            await _uow.SaveChangeAsync(ct);
        }
        return Unit.Value;
    }
}