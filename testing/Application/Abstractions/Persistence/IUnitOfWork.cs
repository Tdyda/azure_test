namespace testing.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangeAsync(CancellationToken ct = default);
}