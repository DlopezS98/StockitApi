using StockitApi.Entities;

namespace StockitApi.Interfaces;

public interface IUnitOfWork
{
    IAsyncRepository<User, int> Users { get; }
    IAsyncRepository<RefreshToken, Guid> RefreshTokens { get; }
    IAsyncRepository<Role, int> Roles { get; }
    IAsyncRepository<ApiUsageLog, Guid> ApiUsageLogs { get; }
    IAsyncRepository<Product, int> Products { get; }
    IAsyncRepository<Category, int> Categories { get; }
    IAsyncRepository<Invoice, int> Invoices { get; }
    IAsyncRepository<InvoiceItem, int> InvoiceItems { get; }
    IAsyncRepository<Customer, int> Customers { get; }
    IAsyncRepository<Warehouse, int> Warehouses { get; }
    IAsyncRepository<Inventory, int> Inventories { get; }
    IAsyncRepository<StockMovement, long> StockMovements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync();
}
