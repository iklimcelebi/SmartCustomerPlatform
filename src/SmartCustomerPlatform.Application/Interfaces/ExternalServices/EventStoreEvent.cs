namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public class EventStoreEvent
{
    public string EventId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string Data { get; set; } = null!;

    public DateTime Created { get; set; }
}