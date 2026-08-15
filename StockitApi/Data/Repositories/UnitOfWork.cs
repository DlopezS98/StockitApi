using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockitContext _context;
    public IAsyncRepository<User, int> Users { get; private set; }

    public UnitOfWork(StockitContext context)
    {
        _context = context;
        Users = new EfRepository<User, int>(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}