using StockitApi.Entities;

namespace StockitApi.Interfaces;

public interface IAsyncRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<bool> ExistsAsync(TKey id);
    Task<List<TEntity>> AddAsync(IEnumerable<TEntity> entity);
    Task<List<TEntity>> DeleteAsync(IEnumerable<TEntity> entity);
    Task<List<TEntity>> UpdateAsync(IEnumerable<TEntity> entity);
    public Task<int> SaveChangesAsync();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}