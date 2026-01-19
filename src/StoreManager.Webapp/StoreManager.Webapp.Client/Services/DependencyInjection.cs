namespace StoreManager.Webapp.Client.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddStoreManagerServices(this IServiceCollection services)
    {
        // Example of adding an HttpClient for a specific service
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IChainService, ChainService>();
        services.AddScoped<NavState>();
        return services;
    }
}
