using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Persistence;
using Testcontainers.MsSql;

namespace StoreManager.API.IntegrationTests;

public class StoreManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder() // MsSqlBuilder from Testcontainers has been marked as obsolete, but will still be used here due Testcontainers not yet having a non-obsolete alternative for MSSQL as of writing (27-01-2026).
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithName($"storemanager_dev.db{Guid.NewGuid():N}")
        .WithPassword("yourStrong(!)Password")
        .WithCleanUp(true)
        .Build();

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
        await context.Database.EnsureCreatedAsync();
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}
