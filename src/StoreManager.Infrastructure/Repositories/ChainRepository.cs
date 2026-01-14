using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
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
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        if (entity.Stores.Any())
        {
            await _dbContext.ChainEntities.AddAsync(entity, cancellationToken);
            await _dbContext.StoreEntities.AddRangeAsync(entity.Stores, cancellationToken);
            Save(cancellationToken);

            await _dbContext.Database.CommitTransactionAsync(cancellationToken);

            return Task.FromResult(entity).Result;
        }

        await _dbContext.ChainEntities.AddAsync(entity, cancellationToken);
        Save(cancellationToken);

        await _dbContext.Database.CommitTransactionAsync(cancellationToken);

        return Task.FromResult(entity).Result;
    }

    public async Task<IEnumerable<ChainEntity>> AddRangeAsync(List<ChainEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        await _dbContext.ChainEntities.AddRangeAsync(entities, cancellationToken);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);

        return await Task.FromResult(entities.AsEnumerable());
    }

    public async Task<ChainEntity> GetByIdAsync(object id)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var result = await _dbContext.ChainEntities.FindAsync(id);
        await _dbContext.Database.CommitTransactionAsync();

        return result;
    }

    public async Task<ChainEntity> GetByIdIncludeStoresAsync(object id)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var result = _dbContext.ChainEntities.AsNoTracking().Include(s => s.Stores).Where(s => s.Id == (ChainId)id);
        var chain = await result.FirstOrDefaultAsync();
        await _dbContext.Database.CommitTransactionAsync();
        
        return chain!;
    }

    public async Task<int> GetCountofStoresByChainAsync(object id)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        int stores = _dbContext.StoreEntities.AsNoTracking().Count(c => c.ChainId! == (ChainId)id);
        await _dbContext.Database.CommitTransactionAsync();

        return stores;
    }

    public async Task UpdateAsync(ChainEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ChainEntities.Attach(entity);
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        _dbContext.ChainEntities.Update(entity);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        var entity = await _dbContext.ChainEntities.FindAsync(id);
        _dbContext.ChainEntities.Remove(entity);
        Save(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public void Save(CancellationToken cancellationToken = default)
    {
        _dbContext.SaveChanges(cancellationToken);
    }
}
