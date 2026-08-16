using System.ComponentModel.DataAnnotations;

namespace StockitApi.Dtos;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    string Barcode,
    string QrCode,
    decimal Price,
    int CategoryId,
    string CategoryName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateProductRequest(
    [Required] string Name,
    string? Description,
    [Required] string Barcode,
    [Required] string QrCode,
    [Range(0, double.MaxValue)] decimal Price,
    [Required] int CategoryId
);

public record UpdateProductRequest(
    [Required] string Name,
    string? Description,
    [Range(0, double.MaxValue)] decimal Price,
    [Required] int CategoryId
);
