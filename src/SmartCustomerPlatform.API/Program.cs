using MediatR;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;
using SmartCustomerPlatform.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblyContaining<CreateSubscriptionCommand>());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();