using System.Text;
using EventStore.Client;

namespace SmartCustomerPlatform.Infrastructure.EventStore;

public class EventStoreService
{
    private readonly EventStoreClient _client;

    public EventStoreService(EventStoreClient client)
    {
        _client = client;
    }

    public async Task AppendAsync(
        Guid aggregateId,
        Guid eventId,
        string eventType,
        string payload,
        CancellationToken cancellationToken = default)
    {
        var streamName = $"subscription-{aggregateId}";

        var eventData = new EventData(
            Uuid.FromGuid(eventId),
            eventType,
            Encoding.UTF8.GetBytes(payload));

        await _client.AppendToStreamAsync(
            streamName,
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken);
    }
}