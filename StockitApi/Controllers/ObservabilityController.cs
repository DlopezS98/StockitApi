using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StockitApi.Authorization;
using StockitApi.Dtos;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Controllers;

[ApiController]
[Route("api")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ObservabilityController(IObservabilityService observabilityService) : ControllerBase
{
    private readonly IObservabilityService _observabilityService = observabilityService;

    [RequirePermission(Permission.Logs.Read)]
    [HttpGet("logs/audit")]
    [ProducesResponseType<IReadOnlyList<AuditLogResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        IReadOnlyList<AuditLogResponse> result = await _observabilityService.GetAuditLogsAsync(page, pageSize);
        return Ok(result);
    }

    [RequirePermission(Permission.Metrics.Read)]
    [HttpGet("metrics/usage")]
    [ProducesResponseType<IReadOnlyList<UsageMetricResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UsageMetrics([FromQuery] DateTime? from)
    {
        IReadOnlyList<UsageMetricResponse> result = await _observabilityService.GetUsageMetricsAsync(from);
        return Ok(result);
    }
}
