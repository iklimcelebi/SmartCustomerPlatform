using Elastic.Clients.Elasticsearch;
using EventStore.Client;
using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;
using SmartCustomerPlatform.Infrastructure.EventStore;
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

var eventStoreConnectionString =
    builder.Configuration["EventStoreDb:ConnectionString"]
    ?? "esdb://localhost:2113?tls=false";

var eventStoreSettings =
    EventStoreClientSettings.Create(
        eventStoreConnectionString);

builder.Services.AddSingleton(
    new EventStoreClient(eventStoreSettings));

builder.Services.AddSingleton<EventStoreReaderService>();

builder.Services.AddScoped<
    ISubscriptionSearchService,
    SubscriptionSearchService>();

builder.Services.AddScoped<
    ISubscriptionProjectionRebuildService,
    SubscriptionProjectionRebuildService>();

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