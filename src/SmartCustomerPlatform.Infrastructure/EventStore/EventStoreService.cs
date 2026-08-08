using System.Text.Json;
using EventStore.Client;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;

namespace SmartCustomerPlatform.Infrastructure.EventStore;

public class EventStoreService : IEventStoreService
{
    private readonly EventStoreClient _client;

    public EventStoreService(EventStoreClient client)
    {
        _client = client;
    }

    public async Task AppendEventAsync(
        string streamName,
        string eventType,
        object eventData,
        CancellationToken cancellationToken = default)
    {
        var eventJson = JsonSerializer.SerializeToUtf8Bytes(eventData);

        var eventDataObject = new EventData(
            Uuid.NewUuid(),
            eventType,
            eventJson);

        await _client.AppendToStreamAsync(
            streamName,
            StreamState.Any,
            new[]
            {
                eventDataObject
            },
            cancellationToken: cancellationToken);
    }
}