using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Store;
using System.Data;

namespace StoreManager.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly IApplicationDbContext _dbContext;
    
    public StoreRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreEntity> AddAsync(StoreEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _dbContext.StoreEntities.AddAsync(entity, cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        Save(cancellationToken);

        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<StoreEntity>> AddRangeAsync(List<StoreEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _dbContext.StoreEntities.AddRangeAsync(entities, cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        Save(cancellationToken);

        return await Task.FromResult(entities);
    }

    public async Task<StoreEntity> GetByIdAsync(object id)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var result = await _dbContext.StoreEntities.FindAsync(id);
        await _dbContext.Database.CommitTransactionAsync();
        
        return result;
    }

    public async Task<IReadOnlyList<StoreEntity>> GetAllByChainIdAsync(object chainId)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var result = await _dbContext.StoreEntities.AsNoTracking()
            .Where(s => s.ChainId == (ChainId)chainId)
            .ToListAsync();
        await _dbContext.Database.CommitTransactionAsync();
        if (result.Count() < 1)
        {
            throw new KeyNotFoundException($"No stores found for ChainId: {chainId}");
        }

        return result;
    }

    public async Task UpdateAsync(StoreEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.StoreEntities.Attach(entity);
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        _dbContext.StoreEntities.Update(entity);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        var entity = await _dbContext.StoreEntities.FindAsync(id);
        _dbContext.StoreEntities.Remove(entity);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task DeleteByChainIdAsync(object chainId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        var entities = _dbContext.StoreEntities.Where(s => s.ChainId == (ChainId)chainId);
        _dbContext.StoreEntities.RemoveRange(entities);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public void Save(CancellationToken cancellationToken = default)
    {
        _dbContext.SaveChanges(cancellationToken);
    }    
}
