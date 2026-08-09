namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public interface IEventStoreService
{
    Task AppendEventAsync(
        string streamName,
        string eventType,
        object eventData,
        CancellationToken cancellationToken = default);

    Task AppendJsonEventAsync(
        string streamName,
        string eventType,
        string json,
        CancellationToken cancellationToken = default);
}