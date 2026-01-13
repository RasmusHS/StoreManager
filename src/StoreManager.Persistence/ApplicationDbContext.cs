using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Data;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Store;

namespace StoreManager.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ChainEntity> ChainEntities { get; set; }
    public DbSet<StoreEntity> StoreEntities { get; set; }

    public void SaveChanges(CancellationToken cancellationToken = default)
    {
        base.SaveChanges();
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Only apply seed data if not in testing environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Testing")
        {
            
        }
    }

    private byte[] ImageToByteArray(string imagePath)
    {
        return File.ReadAllBytes(imagePath);
    }
