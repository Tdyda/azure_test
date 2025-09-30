using testing.Domain.ValueObjects;
using testing.Models;

namespace testing.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByEmail(Email email, CancellationToken ct);
    Task AddAsync(User user, String PasswordHash, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
}