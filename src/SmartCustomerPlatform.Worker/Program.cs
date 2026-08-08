using Elastic.Clients.Elasticsearch;
using EventStore.Client;
using SmartCustomerPlatform.Infrastructure.EventStore;
using SmartCustomerPlatform.Persistence.DependencyInjection;
using SmartCustomerPlatform.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPersistenceServices(builder.Configuration);

// EventStoreDB
var eventStoreConnectionString =
    builder.Configuration["EventStoreDb:ConnectionString"]
    ?? throw new InvalidOperationException(
        "EventStoreDb connection string bulunamadı.");

var eventStoreSettings =
    EventStoreClientSettings.Create(eventStoreConnectionString);

builder.Services.AddSingleton(
    new EventStoreClient(eventStoreSettings));

builder.Services.AddSingleton<EventStoreService>();
builder.Services.AddSingleton<EventStoreReaderService>();

// Elasticsearch
var elasticsearchUrl =
    builder.Configuration["Elasticsearch:Url"]
    ?? throw new InvalidOperationException(
        "Elasticsearch URL bulunamadı.");

var elasticsearchSettings =
    new ElasticsearchClientSettings(new Uri(elasticsearchUrl));

builder.Services.AddSingleton(
    new ElasticsearchClient(elasticsearchSettings));

// Workers
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<ProjectionWorker>();

var host = builder.Build();

host.Run();