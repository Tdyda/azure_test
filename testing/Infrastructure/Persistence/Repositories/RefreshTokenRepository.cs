using Microsoft.EntityFrameworkCore;
using testing.Application.Abstractions;
using testing.Models;

namespace testing.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _db.RefreshTokens.AddAsync(token, ct).AsTask();
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
    {
        return await _db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.RefreshTokens.Where(x => x.UserId == userId).ToListAsync(ct);
    }
}