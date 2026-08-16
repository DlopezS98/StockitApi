using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record InvoiceQueryParameters
{
    // Matches against invoice number or customer name.
    public string? Search { get; init; }
    public string? Status { get; init; }
    public int? CustomerId { get; init; }
    public int? WarehouseId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    [Range(1, int.MaxValue)] public int Page { get; init; } = 1;
    [Range(1, 200)] public int PageSize { get; init; } = 20;
}

public record InvoiceItemRequest(
    [Required] int ProductId,
    [Range(1, int.MaxValue)] int Quantity,
    decimal? UnitPrice,
    [Range(0, double.MaxValue)] decimal DiscountAmount = 0
);

public record CreateInvoiceRequest(
    [Required] int CustomerId,
    [Required] int WarehouseId,
    [MinLength(1)] List<InvoiceItemRequest> Items,
    [Range(0, double.MaxValue)] decimal DiscountAmount = 0,
    string Currency = "USD",
    string? Notes = null,
    DateTime? DueDate = null
);

public record InvoiceItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    decimal TotalAmount
);

public record InvoiceResponse(
    int Id,
    string InvoiceNumber,
    int CustomerId,
    string CustomerName,
    int WarehouseId,
    string WarehouseName,
    int SellerId,
    string SellerName,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    string Status,
    string Currency,
    string? Notes,
    DateTime IssuedAt,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<InvoiceItemResponse> Items
);
