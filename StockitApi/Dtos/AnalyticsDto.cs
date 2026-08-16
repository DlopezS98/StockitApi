namespace StockitApi.Dtos;

public record TopProduct(string ProductName, int UnitsSold, decimal Revenue);

public record AnalyticsSummary(
    decimal TotalRevenue,
    int InvoiceCount,
    decimal AverageInvoiceValue,
    List<TopProduct> TopProducts
);

public record SalesByDay(DateOnly Date, decimal Revenue, int InvoiceCount);