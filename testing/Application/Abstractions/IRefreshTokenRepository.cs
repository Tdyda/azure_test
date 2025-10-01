using testing.Models;

namespace testing.Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId, CancellationToken ct);
}