namespace StockitApi.Entities;

public class InvoiceItem : BaseEntity<int>
{
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public Invoice? Invoice { get; set; }
    public Product? Product { get; set; }
}
