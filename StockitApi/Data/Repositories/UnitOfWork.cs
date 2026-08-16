using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockitContext _context;
    public IAsyncRepository<User, int> Users { get; private set; }
    public IAsyncRepository<RefreshToken, Guid> RefreshTokens { get; private set; }
    public IAsyncRepository<Role, int> Roles { get; private set; }
    public IAsyncRepository<ApiUsageLog, Guid> ApiUsageLogs { get; private set; }

    public UnitOfWork(StockitContext context)
    {
        _context = context;
        Users = new EfRepository<User, int>(context);
        RefreshTokens = new EfRepository<RefreshToken, Guid>(context);
        Roles = new EfRepository<Role, int>(context);
        ApiUsageLogs = new EfRepository<ApiUsageLog, Guid>(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}