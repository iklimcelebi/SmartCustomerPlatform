using SmartCustomerPlatform.Application.DependencyInjection;
using SmartCustomerPlatform.Persistence.DependencyInjection;
using SmartCustomerPlatform.API.Middleware;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.Seed;





var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.


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