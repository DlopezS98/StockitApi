using StockitApi.Dtos;
using StockitApi.Entities;

namespace StockitApi.Specifications;

public class InvoiceWithDetailsSpecification : BaseSpecification<Invoice>
{
    public InvoiceWithDetailsSpecification()
    {
        AddIncludes();
    }

    public InvoiceWithDetailsSpecification(int id)
    {
        ApplyCriteria(i => i.Id == id);
        AddIncludes();
    }

    // Pass applyPaging: false when the same filters are reused to compute a total count.
    public InvoiceWithDetailsSpecification(InvoiceQueryParameters query, bool applyPaging = true)
    {
        string? search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        ApplyCriteria(i =>
            (search == null || i.InvoiceNumber.Contains(search) || i.Customer!.Name.Contains(search)) &&
            (query.Status == null || i.Status == query.Status) &&
            (query.CustomerId == null || i.CustomerId == query.CustomerId) &&
            (query.WarehouseId == null || i.WarehouseId == query.WarehouseId) &&
            (query.From == null || i.IssuedAt >= query.From) &&
            (query.To == null || i.IssuedAt <= query.To));

        AddIncludes();
        ApplyOrderByDescending(i => i.IssuedAt);

        if (applyPaging) ApplyPaging((query.Page - 1) * query.PageSize, query.PageSize);
    }

    private void AddIncludes()
    {
        AddInclude(i => i.Customer!);
        AddInclude(i => i.Warehouse!);
        AddInclude(i => i.Seller!);
        AddInclude("InvoiceItems.Product");
    }
}

