namespace StockitApi.Entities;

public class Product : BaseEntity<int>
{
    required public string Name { get; set; }
    public string? Description { get; set; }
    required public string Barcode { get; set; }
    required public string QrCode { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }

    public Category? Category { get; set; }
    public ICollection<Inventory> Inventories { get; set; } = [];
    public ICollection<StockMovement> StockMovements { get; set; } = [];
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = [];
}
