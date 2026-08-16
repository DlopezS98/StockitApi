namespace StockitApi.Dtos;

public record AuditLogResponse(Guid Id, string? Username, string Action, string EntityType, int EntityId, DateTime Timestamp);

public record UsageMetricResponse(string Endpoint, int RequestCount, double AverageResponseTimeMs, int ErrorCount);
