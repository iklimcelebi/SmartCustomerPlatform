using Elastic.Clients.Elasticsearch;
using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;
using SmartCustomerPlatform.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblyContaining<CreateSubscriptionCommand>());

var elasticsearchUrl =
    builder.Configuration["Elasticsearch:Url"]
    ?? "http://localhost:9200";

var elasticsearchSettings =
    new ElasticsearchClientSettings(
        new Uri(elasticsearchUrl));

builder.Services.AddSingleton(
    new ElasticsearchClient(elasticsearchSettings));

builder.Services.AddScoped<
    ISubscriptionSearchService,
    SubscriptionSearchService>();

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