using Microsoft.Extensions.DependencyInjection;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Infrastructure.Repositories;

namespace StoreManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //services.AddScoped<,>();
        services.AddScoped<IChainRepository, ChainRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();

        return services;
    }
}
