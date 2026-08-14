namespace StockitApi.Entities;

public class Customer : BaseEntity<int>
{
    required public string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = [];
}
