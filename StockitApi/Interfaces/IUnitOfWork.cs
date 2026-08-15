using StockitApi.Entities;

namespace StockitApi.Interfaces;

public interface IUnitOfWork
{
    IAsyncRepository<User, int> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync();
}
