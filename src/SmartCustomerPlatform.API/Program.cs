using SmartCustomerPlatform.Application.DependencyInjection;
using SmartCustomerPlatform.Persistence.DependencyInjection;
using SmartCustomerPlatform.API.Middleware;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Seed;
using SmartCustomerPlatform.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build application
var app = builder.Build();

// Configure HTTP request pipeline
app.UseCors("Frontend");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<SmartCustomerPlatformDbContext>();
    await DepartmentSeed.SeedAsync(context);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();