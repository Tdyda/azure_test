using testing.Application.Abstractions.Persistence;

namespace testing.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
    }
    public Task<int> SaveChangeAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}