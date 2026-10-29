using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockitContext _context;
    public IAsyncRepository<User, int> Users { get; private set; }
    public IAsyncRepository<RefreshToken, Guid> RefreshTokens { get; private set; }
    public IAsyncRepository<Role, int> Roles { get; private set; }
    public IAsyncRepository<Permission, int> Permissions { get; private set; }
    public IAsyncRepository<ApiUsageLog, Guid> ApiUsageLogs { get; private set; }
    public IAsyncRepository<Product, int> Products { get; private set; }
    public IAsyncRepository<Category, int> Categories { get; private set; }
    public IAsyncRepository<Invoice, int> Invoices { get; private set; }
    public IAsyncRepository<InvoiceItem, int> InvoiceItems { get; private set; }
    public IAsyncRepository<Customer, int> Customers { get; private set; }
    public IAsyncRepository<Warehouse, int> Warehouses { get; private set; }
    public IAsyncRepository<Inventory, int> Inventories { get; private set; }
    public IAsyncRepository<StockMovement, long> StockMovements { get; private set; }
    public IAsyncRepository<AuditLog, Guid> AuditLogs { get; private set; }

    public UnitOfWork(StockitContext context)
    {
        _context = context;
        Users = new EfRepository<User, int>(context);
        RefreshTokens = new EfRepository<RefreshToken, Guid>(context);
        Roles = new EfRepository<Role, int>(context);
        Permissions = new EfRepository<Permission, int>(context);
        ApiUsageLogs = new EfRepository<ApiUsageLog, Guid>(context);
        Products = new EfRepository<Product, int>(context);
        Categories = new EfRepository<Category, int>(context);
        Invoices = new EfRepository<Invoice, int>(context);
        InvoiceItems = new EfRepository<InvoiceItem, int>(context);
        Customers = new EfRepository<Customer, int>(context);
        Warehouses = new EfRepository<Warehouse, int>(context);
        Inventories = new EfRepository<Inventory, int>(context);
        StockMovements = new EfRepository<StockMovement, long>(context);
        AuditLogs = new EfRepository<AuditLog, Guid>(context);
    }

    public void RemoveRolePermissions(IEnumerable<RolePermission> rolePermissions)
    {
        _context.RolePermissions.RemoveRange(rolePermissions);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task ExecuteInTransactionAsync(Func<Task> operation)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await operation();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}