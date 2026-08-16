using StockitApi.Dtos;

namespace StockitApi.Interfaces;

public interface IProductService
{
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<ProductResponse>> GetAllAsync();
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
    Task DeactivateAsync(int id);
}
