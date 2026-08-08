using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Infrastructure.EventStore;

namespace SmartCustomerPlatform.Infrastructure.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var eventStoreConnectionString =
            configuration["EventStore:ConnectionString"]
            ?? "esdb://localhost:2113?tls=false";

        var settings = EventStoreClientSettings.Create(
            eventStoreConnectionString);

        var eventStoreClient = new EventStoreClient(settings);

        services.AddSingleton(eventStoreClient);

        services.AddSingleton<IEventStoreService, EventStoreService>();

        return services;
    }
}