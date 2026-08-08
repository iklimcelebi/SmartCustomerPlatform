namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public interface IEventStoreService
{
    Task AppendEventAsync(
        string streamName,
        string eventType,
        object eventData,
        CancellationToken cancellationToken = default);
}