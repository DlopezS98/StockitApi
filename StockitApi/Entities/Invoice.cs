namespace StockitApi.Entities;

public class Invoice : BaseEntity<int>
{
    // e.g., INV-2026-000123
    required public string InvoiceNumber { get; set; }
    public int CustomerId { get; set; }
    public int WarehouseId { get; set; }
    public int SellerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    // e.g., "Pending", "Paid", "Refunded", "Partially Paid", "Overdue", "Void"
    public string Status { get; set; } = "Pending";
    public string Currency { get; set; } = "USD";
    public string? Notes { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
    public Warehouse? Warehouse { get; set; }
    public User? Seller { get; set; }
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
