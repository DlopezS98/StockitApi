namespace StockitApi.Entities;

public class StockMovement : BaseEntity<long>
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }

    // e.g., "Sale", "Restock", "Transfer", "Adjustment", "Return"
    required public string MovementType { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
    public User? CreatedByUser { get; set; }
}

public static class StockMovementType 
{
    public const string Sale = "Sale";
    public const string Restock = "Restock";
    public const string Transfer = "Transfer";
    public const string Adjustment = "Adjustment";
    public const string Return = "Return";

    public static readonly HashSet<string> All =
    [
        Sale,
        Restock,
        Transfer,
        Adjustment,
        Return
    ];
}