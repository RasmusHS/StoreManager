using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Application.Data;

namespace StoreManager.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
