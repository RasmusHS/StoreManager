using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Application.Data;

namespace StoreManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IDispatcher>(d => new Dispatcher(d.GetService<IMediator>()));

        return services;
    }
}
