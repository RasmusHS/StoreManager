using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Persistence;
using Testcontainers.MsSql;
using Testcontainers;
using DotNet.Testcontainers.Images;

namespace StoreManager.API.IntegrationTests;

public class StoreManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    //private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
    //    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    //    .WithName($"storemanager_dev.db{Guid.NewGuid():N}")
    //    .WithPassword("yourStrong(!)Password")
    //    .WithCleanUp(true)
    //    .Build();

    private readonly MsSqlContainer _dbContainer = new MsSqlContainer(new MsSqlConfiguration($"storemanager_dev.db{Guid.NewGuid():N}", "SA", "yourStrong(!)Password"));

    // Track if THIS factory's database has been initialized
    private bool _databaseInitialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseSqlServer(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention()
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
        });
    }

    public async Task EnsureDatabaseCreatedAsync(ApplicationDbContext context)
    {
        if (!_databaseInitialized)
        {
            await _initLock.WaitAsync();
            try
            {
                if (!_databaseInitialized)
                {
                    await context.Database.EnsureCreatedAsync();
                    _databaseInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}
