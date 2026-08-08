using EventStore.Client;

namespace SmartCustomerPlatform.Infrastructure.EventStore;

public class EventStoreReaderService
{
    private readonly EventStoreClient _client;

    public EventStoreReaderService(EventStoreClient client)
    {
        _client = client;
    }

    public EventStoreClient Client => _client;
}