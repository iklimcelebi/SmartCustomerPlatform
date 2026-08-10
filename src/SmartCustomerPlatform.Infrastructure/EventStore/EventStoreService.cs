using System.Text;
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
        var eventJson =
            JsonSerializer.SerializeToUtf8Bytes(eventData);

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

    public async Task AppendJsonEventAsync(
        string streamName,
        string eventType,
        string json,
        CancellationToken cancellationToken = default)
    {
        var eventDataObject = new EventData(
            Uuid.NewUuid(),
            eventType,
            Encoding.UTF8.GetBytes(json));

        await _client.AppendToStreamAsync(
            streamName,
            StreamState.Any,
            new[]
            {
                eventDataObject
            },
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<EventStoreEventDto>> GetEventsAsync(
        string streamName,
        CancellationToken cancellationToken = default)
    {
        var result = new List<EventStoreEventDto>();

        try
        {
            var events = _client.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start,
                cancellationToken: cancellationToken);

            await foreach (
                var resolvedEvent in events
                    .WithCancellation(cancellationToken))
            {
                var recordedEvent = resolvedEvent.Event;

                var data = Encoding.UTF8.GetString(
                    recordedEvent.Data.Span);

                result.Add(
                    new EventStoreEventDto(
                        recordedEvent.EventNumber.ToUInt64(),
                        recordedEvent.EventType,
                        recordedEvent.Created,
                        data));
            }
        }
        catch (StreamNotFoundException)
        {
            return Array.Empty<EventStoreEventDto>();
        }

        return result;
    }

    public async Task<IReadOnlyList<EventStoreEventDto>> GetAllEventsAsync(
        CancellationToken cancellationToken = default)
    {
        var result = new List<EventStoreEventDto>();

        var events = _client.ReadAllAsync(
            Direction.Forwards,
            Position.Start,
            cancellationToken: cancellationToken);

        await foreach (
            var resolvedEvent in events
                .WithCancellation(cancellationToken))
        {
            var recordedEvent = resolvedEvent.Event;

            if (!recordedEvent.EventStreamId.StartsWith(
                    "ticket-",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var data = Encoding.UTF8.GetString(
                recordedEvent.Data.Span);

            result.Add(
                new EventStoreEventDto(
                    recordedEvent.EventNumber.ToUInt64(),
                    recordedEvent.EventType,
                    recordedEvent.Created,
                    data));
        }

        return result;
    }














}


