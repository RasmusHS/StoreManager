using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Persistence;

namespace StoreManager.API.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<StoreManagerWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly StoreManagerWebApplicationFactory _factory;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(StoreManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        InitializeDatabaseAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeDatabaseAsync()
    {
        // Factory ensures database is created once per container
        await _factory.EnsureDatabaseCreatedAsync(DbContext);

        // Clean data for this test
        await CleanDatabaseAsync();
    }

    private async Task CleanDatabaseAsync()
    {
        // Remove all data from tables
        DbContext.StoreEntities.RemoveRange(DbContext.StoreEntities);
        DbContext.ChainEntities.RemoveRange(DbContext.ChainEntities);

        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (DbContext != null)
            await DbContext.DisposeAsync();

        _scope?.Dispose();
    }
}
