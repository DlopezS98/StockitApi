namespace StockitApi.Entities;

public class Payment : BaseEntity<int>
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }

    // e.g., "Cash", "Credit Card", "Bank Transfer", "PayPal", "Check"
    required public string PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? PaidAt { get; set; }

    public Invoice? Invoice { get; set; }
    public User? CreatedByUser { get; set; }
}
