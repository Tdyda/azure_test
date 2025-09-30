using Microsoft.EntityFrameworkCore;
using testing.Application.Abstractions.Persistence;
using testing.Domain.ValueObjects;
using testing.Models;

namespace testing.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsByEmail(Email email, CancellationToken ct)
    {
        return _db.Users.AnyAsync(u => u.Email.Value == email.Value, ct);
    }

    public async Task AddAsync(User user, string PasswordHash, CancellationToken ct)
    {
        User.SetPassword(user, PasswordHash);
        await _db.Users.AddAsync(user, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return _db.Users.FirstOrDefaultAsync(x => x.Email.Value == email, ct);
    }
}