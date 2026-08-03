using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.DependencyInjection;

public static class ServiceRegistration // this is a static class. ıts job is holding helper methods.
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,// this is a extension method.
        IConfiguration configuration)
    {
        services.AddDbContext<SmartCustomerPlatformDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"))); // this will read the connection from appsettings.json file.

        return services;
    }
}