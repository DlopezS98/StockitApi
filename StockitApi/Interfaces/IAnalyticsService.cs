using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsSummary> GetSummaryAsync(DateTime? from, DateTime? to);
    Task<IReadOnlyList<SalesByDay>> GetSalesByDayAsync(DateTime from, DateTime to);
}
