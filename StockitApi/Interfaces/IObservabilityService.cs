using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IObservabilityService
{
    Task<IReadOnlyList<AuditLogResponse>> GetAuditLogsAsync(int page, int pageSize);
    Task<IReadOnlyList<UsageMetricResponse>> GetUsageMetricsAsync(DateTime? from);
}
