using Microsoft.EntityFrameworkCore;
using StockitApi.Data;
using StockitApi.Dtos;
using StockitApi.Interfaces;

namespace StockitApi.Services;

// Uses the DbContext directly since these paging/group-by queries don't fit the repository/specification pattern.
public class ObservabilityService(StockitContext context) : IObservabilityService
{
    private readonly StockitContext _context = context;

    public async Task<IReadOnlyList<AuditLogResponse>> GetAuditLogsAsync(int page, int pageSize)
    {
        List<Entities.AuditLog> logs = await _context.AuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        List<int> userIds = [.. logs.Select(l => l.UserId).Distinct()];
        Dictionary<int, string> users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username);

        return [.. logs.Select(l => new AuditLogResponse(
            l.Id, users.GetValueOrDefault(l.UserId), l.Action, l.EntityType, l.EntityId, l.Timestamp))];
    }

    public async Task<IReadOnlyList<UsageMetricResponse>> GetUsageMetricsAsync(DateTime? from)
    {
        IQueryable<Entities.ApiUsageLog> logs = _context.ApiUsageLogs;
        if (from.HasValue) logs = logs.Where(l => l.CreatedAt >= from.Value);

        // Aggregates are computed per group in-memory - the DTO constructor call itself can't be translated to SQL.
        var grouped = await logs
            .GroupBy(l => l.Endpoint)
            .Select(g => new
            {
                Endpoint = g.Key,
                RequestCount = g.Count(),
                AverageResponseTimeMs = g.Average(l => l.ResponseTimeMs),
                ErrorCount = g.Count(l => l.StatusCode >= 400),
            })
            .ToListAsync();

        return [.. grouped
            .OrderByDescending(m => m.RequestCount)
            .Select(m => new UsageMetricResponse(m.Endpoint, m.RequestCount, m.AverageResponseTimeMs, m.ErrorCount))];
    }
}
