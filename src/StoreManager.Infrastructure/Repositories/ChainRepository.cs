using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Store;
using System.Data;

namespace StoreManager.Infrastructure.Repositories;

public class ChainRepository : IChainRepository
{
    private readonly IApplicationDbContext _dbContext;

    public ChainRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChainEntity> AddAsync(ChainEntity entity, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            if (entity.Stores.Any())
            {
                await _dbContext.ChainEntities.AddAsync(entity, cancellationToken);
                await _dbContext.StoreEntities.AddRangeAsync(entity.Stores, cancellationToken);
            }
            else
            {
                await _dbContext.ChainEntities.AddAsync(entity, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return entity;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<ChainEntity>> AddRangeAsync(List<ChainEntity> entities, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            await _dbContext.ChainEntities.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return entities;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ChainEntity?> GetByIdAsync(object id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var result = await _dbContext.ChainEntities.FindAsync(id);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ChainEntity?> GetByIdIncludeStoresAsync(object id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var chainIdValue = id as ChainId ?? throw new ArgumentException("Invalid ChainId", nameof(id));

            var chain = await _dbContext.ChainEntities
                .AsNoTracking()
                .Include(s => s.Stores)
                .FirstOrDefaultAsync(s => s.Id == chainIdValue);

            await transaction.CommitAsync();
            return chain;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> GetCountofStoresByChainAsync(object id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var chainIdValue = id as ChainId ?? throw new ArgumentException("Invalid ChainId", nameof(id));

            int stores = await _dbContext.StoreEntities
                .AsNoTracking()
                .CountAsync(c => c.ChainId != null && c.ChainId == chainIdValue);

            await transaction.CommitAsync();
            return stores;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(ChainEntity entity, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            _dbContext.ChainEntities.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    //public async Task AddStoresToChainAsync(ChainEntity chain, List<StoreEntity> stores, CancellationToken cancellationToken = default)
    //{
    //    await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    //    try
    //    {
    //        //chain.AddRangeStoresToChain(stores);
    //        _dbContext.ChainEntities.Update(chain);
    //        _dbContext.StoreEntities.UpdateRange(stores);
    //        await _dbContext.SaveChangesAsync(cancellationToken);
    //        await transaction.CommitAsync(cancellationToken);
    //    }
    //    catch
    //    {
    //        await transaction.RollbackAsync(cancellationToken);
    //        throw;
    //    }
    //}

    public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        try
        {
            var entity = await _dbContext.ChainEntities.FindAsync(id);
            if (entity is null)
            {
                throw new KeyNotFoundException($"ChainEntity with id {id} not found.");
            }

            _dbContext.ChainEntities.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public void Save(CancellationToken cancellationToken = default)
    {
        _dbContext.SaveChanges(cancellationToken);
    }
}
