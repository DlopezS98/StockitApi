using StockitApi.Entities;

namespace StockitApi.Specifications;

public class ProductWithCategorySpecification : BaseSpecification<Product>
{
    public ProductWithCategorySpecification()
    {
        AddInclude(p => p.Category!);
    }

    public ProductWithCategorySpecification(int id)
    {
        ApplyCriteria(p => p.Id == id);
        AddInclude(p => p.Category!);
    }

    public ProductWithCategorySpecification(string code)
    {
        ApplyCriteria(p => p.Barcode == code || p.QrCode == code);
        AddInclude(p => p.Category!);
    }
}
