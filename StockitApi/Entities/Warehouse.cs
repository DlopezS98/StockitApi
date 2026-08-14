namespace StockitApi.Entities;

public class Warehouse : BaseEntity<int>
{
    required public string Name { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = [];
    public ICollection<StockMovement> StockMovements { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
