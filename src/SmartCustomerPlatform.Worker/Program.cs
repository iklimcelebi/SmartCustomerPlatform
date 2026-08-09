using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Infrastructure.DependencyInjection;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SmartCustomerPlatformDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString("DefaultConnection")));

builder.Services.AddInfrastructureServices(
    builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();