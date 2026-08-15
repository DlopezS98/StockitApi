using StockitApi.Interfaces;
using StockitApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace StockitApi.Data.Repositories;

public class EfRepository<TEntity, Tkey>(StockitContext context) 
    : IAsyncRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
{
    private readonly StockitContext _context = context;

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public async Task<List<TEntity>> AddAsync(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
        return entities.ToList();
    }

    public Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<TEntity>> DeleteAsync(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        return [.. entities];
    }

    public async Task<bool> ExistsAsync(Tkey id)
    {
        return await _context.Set<TEntity>().FindAsync(id) is not null;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Tkey id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task<List<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().UpdateRange(entities);
        return Task.FromResult(entities.ToList());
    }
}