using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Repositories;

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

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        services.AddScoped<ITicketCategoryRepository, TicketCategoryRepository>();

        services.AddScoped<ITicketSubCategoryRepository, TicketSubCategoryRepository>();

        services.AddScoped<ITicketRepository, TicketRepository>();
        
        services.AddScoped<ICommentRepository, CommentRepository>();
        return services;
    }
}