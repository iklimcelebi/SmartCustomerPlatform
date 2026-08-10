using Elastic.Clients.Elasticsearch;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Infrastructure.Elasticsearch;
using SmartCustomerPlatform.Infrastructure.EventStore;
using SmartCustomerPlatform.Infrastructure.Projections;

namespace SmartCustomerPlatform.Infrastructure.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EventStoreDB
        var eventStoreConnectionString =
            configuration.GetConnectionString("EventStore")
            ?? "esdb://localhost:2113?tls=false";

        var eventStoreSettings =
            EventStoreClientSettings.Create(eventStoreConnectionString);

        var eventStoreClient =
            new EventStoreClient(eventStoreSettings);

        services.AddSingleton(eventStoreClient);
        services.AddSingleton<IEventStoreService, EventStoreService>();

        // Elasticsearch
        var elasticsearchUrl =
            configuration.GetConnectionString("Elasticsearch")
            ?? "http://localhost:9200";

        var elasticsearchSettings =
            new ElasticsearchClientSettings(
                new Uri(elasticsearchUrl));

        var elasticsearchClient =
            new ElasticsearchClient(elasticsearchSettings);

        services.AddSingleton(elasticsearchClient);
        services.AddSingleton<IElasticsearchService, ElasticsearchService>();

    // Projection
    services.AddScoped<IProjectionService, TicketProjectionService>();

        return services;
    }
}