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

    private void AddIncludes()
    {
        AddInclude(i => i.Customer!);
        AddInclude(i => i.Warehouse!);
        AddInclude(i => i.Seller!);
        AddInclude("InvoiceItems.Product");
    }
}
