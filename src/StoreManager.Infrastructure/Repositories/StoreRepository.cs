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

        return result;
    }

    public async Task<IReadOnlyList<StoreEntity>> GetAllIndependentStoresAsync()
    {
        return await _dbContext.StoreEntities
            .AsNoTracking()
            .Where(s => s.ChainId == null)
            .ToListAsync();
    }

    public async Task UpdateAsync(StoreEntity entity, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try 
        {
            
            //_dbContext.StoreEntities.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (_dbContext.StoreEntities.Entry(entity).Context.ChangeTracker.Entries().Count() == 0) // Check how many rows were affected
            {
                throw new DbUpdateConcurrencyException("The store was modified by another process.");
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
