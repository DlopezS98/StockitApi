using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StockitApi.Authorization;
using StockitApi.Dtos;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    private readonly IAnalyticsService _analyticsService = analyticsService;

    [RequirePermission(Permission.Analytics.Read)]
    [HttpGet("summary")]
    [ProducesResponseType<AnalyticsSummary>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Summary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        AnalyticsSummary result = await _analyticsService.GetSummaryAsync(from, to);
        return Ok(result);
    }

    [RequirePermission(Permission.Analytics.Read)]
    [HttpGet("sales-by-day")]
    [ProducesResponseType<IReadOnlyList<SalesByDay>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SalesByDay([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        IReadOnlyList<SalesByDay> result = await _analyticsService.GetSalesByDayAsync(from, to);
        return Ok(result);
    }
}
