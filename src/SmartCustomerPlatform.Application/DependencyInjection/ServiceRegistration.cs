using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SmartCustomerPlatform.Application.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // assembly means the compliled code of the project.
        // thanks to this we dont have to write the code one by one.
        return services;
    }
}
