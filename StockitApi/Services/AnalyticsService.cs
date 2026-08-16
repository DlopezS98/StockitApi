using Microsoft.EntityFrameworkCore;
using StockitApi.Data;
using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;

namespace StockitApi.Services;

// Uses the DbContext directly since these aggregate/group-by queries don't fit the repository/specification pattern.
public class AnalyticsService(StockitContext context) : IAnalyticsService
{
    private readonly StockitContext _context = context;

    public async Task<AnalyticsSummary> GetSummaryAsync(DateTime? from, DateTime? to)
    {
        IQueryable<Invoice> invoices = _context.Invoices.Where(i => i.Status != InvoiceStatus.Void);
        if (from.HasValue) invoices = invoices.Where(i => i.IssuedAt >= from.Value);
        if (to.HasValue) invoices = invoices.Where(i => i.IssuedAt <= to.Value);

        List<Invoice> list = await invoices.ToListAsync();
        decimal totalRevenue = list.Sum(i => i.TotalAmount);
        int invoiceCount = list.Count;
        decimal averageInvoiceValue = invoiceCount == 0 ? 0 : totalRevenue / invoiceCount;

        List<int> invoiceIds = [.. list.Select(i => i.Id)];
        List<TopProduct> topProducts = await _context.InvoiceItems
            .Where(ii => invoiceIds.Contains(ii.InvoiceId))
            .GroupBy(ii => ii.Product!.Name)
            .Select(g => new TopProduct(g.Key, g.Sum(ii => ii.Quantity), g.Sum(ii => ii.TotalAmount)))
            .OrderByDescending(tp => tp.Revenue)
            .Take(5)
            .ToListAsync();

        return new AnalyticsSummary(totalRevenue, invoiceCount, averageInvoiceValue, topProducts);
    }

    public async Task<IReadOnlyList<SalesByDay>> GetSalesByDayAsync(DateTime from, DateTime to)
    {
        List<Invoice> invoices = await _context.Invoices
            .Where(i => i.Status != InvoiceStatus.Void && i.IssuedAt >= from && i.IssuedAt <= to)
            .ToListAsync();

        return [.. invoices
            .GroupBy(i => DateOnly.FromDateTime(i.IssuedAt))
            .Select(g => new SalesByDay(g.Key, g.Sum(i => i.TotalAmount), g.Count()))
            .OrderBy(s => s.Date)];
    }
}
