using StockitApi.Dtos;
using StockitApi.Entities;
using StockitApi.Interfaces;
using StockitApi.Specifications;

namespace StockitApi.Services;

public class ProductService(
    IUnitOfWork unitOfWork,
    IAppRequestContext appRequestContext,
    TimeProvider timeProvider) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAppRequestContext _appRequestContext = appRequestContext;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        Product? product = await _unitOfWork.Products.GetFirstAsync(new ProductWithCategorySpecification(id));
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse?> LookupAsync(string code)
    {
        Product? product = await _unitOfWork.Products.GetFirstAsync(new ProductWithCategorySpecification(code));
        return product is null ? null : ToResponse(product);
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync()
    {
        IReadOnlyList<Product> products = await _unitOfWork.Products.ListAsync(new ProductWithCategorySpecification());
        return [.. products.Select(ToResponse)];
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        Category category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId)
            ?? throw new InvalidOperationException("Category not found.");

        if (await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Barcode == request.Barcode) is not null)
            throw new InvalidOperationException("A product with this barcode already exists.");
        if (await _unitOfWork.Products.FirstOrDefaultAsync(p => p.QrCode == request.QrCode) is not null)
            throw new InvalidOperationException("A product with this QR code already exists.");

        Product product = new()
        {
            Name = request.Name,
            Description = request.Description,
            Barcode = request.Barcode,
            QrCode = request.QrCode,
            Price = request.Price,
            CategoryId = category.Id,
            Category = category,
            IsActive = true,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
            CreatedBy = _appRequestContext.GetCurrentUserId(),
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
    {
        Product product = await _unitOfWork.Products.GetFirstAsync(new ProductWithCategorySpecification(id))
            ?? throw new KeyNotFoundException("Product not found.");
        Category category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId)
            ?? throw new InvalidOperationException("Category not found.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = category.Id;
        product.Category = category;
        product.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ToResponse(product);
    }

    public async Task DeactivateAsync(int id)
    {
        Product product = await _unitOfWork.Products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product not found.");
        product.IsActive = false;
        product.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Barcode,
            product.QrCode,
            product.Price,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}
