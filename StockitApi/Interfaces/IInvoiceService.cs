using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse?> GetByIdAsync(int id);
    Task<PagedResult<InvoiceResponse>> GetAllAsync(InvoiceQueryParameters query);
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request);
    Task<InvoiceResponse> VoidAsync(int id);
}
