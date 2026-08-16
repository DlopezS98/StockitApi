using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Specifications;

namespace StockitApi.Services;

public class InvoiceService(
    IUnitOfWork unitOfWork,
    IAppRequestContext appRequestContext,
    TimeProvider timeProvider) : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAppRequestContext _appRequestContext = appRequestContext;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<InvoiceResponse?> GetByIdAsync(int id)
    {
        Invoice? invoice = await _unitOfWork.Invoices.GetFirstAsync(new InvoiceWithDetailsSpecification(id));
        return invoice is null ? null : ToResponse(invoice);
    }

    public async Task<IReadOnlyList<InvoiceResponse>> GetAllAsync()
    {
        IReadOnlyList<Invoice> invoices = await _unitOfWork.Invoices.ListAsync(new InvoiceWithDetailsSpecification());
        return [.. invoices.Select(ToResponse)];
    }

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request)
    {
        if (request.Items.Count == 0) throw new ArgumentException("An invoice requires at least one item.");

        Customer customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId)
            ?? throw new InvalidOperationException("Customer not found.");
        Warehouse warehouse = await _unitOfWork.Warehouses.GetByIdAsync(request.WarehouseId)
            ?? throw new InvalidOperationException("Warehouse not found.");
        int sellerId = _appRequestContext.GetCurrentUserId();
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        List<InvoiceItem> invoiceItems = [];
        List<(Inventory Inventory, int Quantity, Product Product)> stockDeductions = [];
        decimal subtotal = 0;
        decimal itemDiscounts = 0;

        foreach (InvoiceItemRequest item in request.Items)
        {
            Product product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");
            Inventory inventory = await _unitOfWork.Inventories.FirstOrDefaultAsync(
                inv => inv.ProductId == item.ProductId && inv.WarehouseId == request.WarehouseId)
                ?? throw new InvalidOperationException($"No inventory record for product '{product.Name}' in the selected warehouse.");
            if (inventory.Quantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'.");

            decimal unitPrice = item.UnitPrice ?? product.Price;
            decimal totalAmount = (unitPrice * item.Quantity) - item.DiscountAmount;

            invoiceItems.Add(new InvoiceItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                DiscountAmount = item.DiscountAmount,
                TotalAmount = totalAmount,
            });
            stockDeductions.Add((inventory, item.Quantity, product));

            subtotal += unitPrice * item.Quantity;
            itemDiscounts += item.DiscountAmount;
        }

        decimal totalDiscount = itemDiscounts + request.DiscountAmount;
        Invoice invoice = new()
        {
            InvoiceNumber = $"PENDING-{Guid.NewGuid():N}",
            CustomerId = customer.Id,
            Customer = customer,
            WarehouseId = warehouse.Id,
            Warehouse = warehouse,
            SellerId = sellerId,
            Subtotal = subtotal,
            DiscountAmount = totalDiscount,
            TotalAmount = subtotal - totalDiscount,
            Status = InvoiceStatus.Pending,
            Currency = request.Currency,
            Notes = request.Notes,
            IssuedAt = now,
            DueDate = request.DueDate,
            CreatedAt = now,
            InvoiceItems = invoiceItems,
        };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            // The invoice number embeds the generated Id, so it can only be assigned after the first save.
            invoice.InvoiceNumber = $"INV-{now:yyyy}-{invoice.Id:D6}";
            await _unitOfWork.Invoices.UpdateAsync(invoice);

            foreach ((Inventory inventory, int quantity, Product product) in stockDeductions)
            {
                inventory.Quantity -= quantity;
                await _unitOfWork.Inventories.UpdateAsync(inventory);
                await _unitOfWork.StockMovements.AddAsync(new StockMovement
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = -quantity,
                    Reason = $"Invoice {invoice.InvoiceNumber}",
                    MovementType = StockMovementType.Sale,
                    MovementDate = now,
                    CreatedBy = sellerId,
                });
            }

            await _unitOfWork.SaveChangesAsync();
        });

        return ToResponse(invoice);
    }

    public async Task<InvoiceResponse> VoidAsync(int id)
    {
        Invoice invoice = await _unitOfWork.Invoices.GetFirstAsync(new InvoiceWithDetailsSpecification(id))
            ?? throw new KeyNotFoundException("Invoice not found.");
        if (invoice.Status == InvoiceStatus.Void) throw new InvalidOperationException("Invoice is already void.");

        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            invoice.Status = InvoiceStatus.Void;
            invoice.UpdatedAt = now;
            await _unitOfWork.Invoices.UpdateAsync(invoice);

            foreach (InvoiceItem item in invoice.InvoiceItems)
            {
                Inventory? inventory = await _unitOfWork.Inventories.FirstOrDefaultAsync(
                    inv => inv.ProductId == item.ProductId && inv.WarehouseId == invoice.WarehouseId);
                if (inventory is null) continue;

                inventory.Quantity += item.Quantity;
                await _unitOfWork.Inventories.UpdateAsync(inventory);
                await _unitOfWork.StockMovements.AddAsync(new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = invoice.WarehouseId,
                    Quantity = item.Quantity,
                    Reason = $"Void of invoice {invoice.InvoiceNumber}",
                    MovementType = StockMovementType.Return,
                    MovementDate = now,
                    CreatedBy = invoice.SellerId,
                });
            }

            await _unitOfWork.SaveChangesAsync();
        });

        return ToResponse(invoice);
    }

    private static InvoiceResponse ToResponse(Invoice invoice)
    {
        List<InvoiceItemResponse> items = [.. invoice.InvoiceItems.Select(item => new InvoiceItemResponse(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.DiscountAmount,
            item.TotalAmount
        ))];

        return new InvoiceResponse(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.CustomerId,
            invoice.Customer?.Name ?? string.Empty,
            invoice.WarehouseId,
            invoice.Warehouse?.Name ?? string.Empty,
            invoice.SellerId,
            invoice.Seller?.FullName ?? string.Empty,
            invoice.Subtotal,
            invoice.DiscountAmount,
            invoice.TotalAmount,
            invoice.Status,
            invoice.Currency,
            invoice.Notes,
            invoice.IssuedAt,
            invoice.DueDate,
            invoice.CreatedAt,
            invoice.UpdatedAt,
            items
        );
    }
}
