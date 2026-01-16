using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Data;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }

    public DbSet<ChainEntity> ChainEntities { get; set; }
    public DbSet<StoreEntity> StoreEntities { get; set; }

    //savechangesasync
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    void SaveChanges(CancellationToken cancellationToken = default);
}
