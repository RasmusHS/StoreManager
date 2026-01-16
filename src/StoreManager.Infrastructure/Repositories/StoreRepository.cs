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
        await _dbContext.StoreEntities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<StoreEntity>> AddRangeAsync(List<StoreEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.StoreEntities.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<StoreEntity?> GetByIdAsync(object id)
    {
        return await _dbContext.StoreEntities.FindAsync(id);
    }

    public async Task<IReadOnlyList<StoreEntity>> GetAllByChainIdAsync(object chainId)
    {
        var chainIdValue = chainId as ChainId ?? throw new ArgumentException("Invalid ChainId", nameof(chainId));

        var result = await _dbContext.StoreEntities.AsNoTracking()
            .Where(s => s.ChainId != null && s.ChainId == chainIdValue)
            .ToListAsync();

        if (result.Count < 1)
        {
            throw new KeyNotFoundException($"No stores found for ChainId: {chainId}");
        }

        return result;
    }

    public async Task UpdateAsync(StoreEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.StoreEntities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.StoreEntities.FindAsync(id);
        if (entity is null)
        {
            throw new KeyNotFoundException($"StoreEntity with id {id} not found.");
        }

        _dbContext.StoreEntities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByChainIdAsync(object chainId, CancellationToken cancellationToken = default)
    {
        var chainIdValue = chainId as ChainId ?? throw new ArgumentException("Invalid ChainId", nameof(chainId));

        var entities = await _dbContext.StoreEntities
            .Where(s => s.ChainId != null && s.ChainId == chainIdValue)
            .ToListAsync(cancellationToken);

        _dbContext.StoreEntities.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Save(CancellationToken cancellationToken = default)
    {
        _dbContext.SaveChanges(cancellationToken);
    }    
}
