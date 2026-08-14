namespace StockitApi.Entities;

public class Category : BaseEntity<int>
{
    required public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }
    public ICollection<Category> ChildCategories { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}
