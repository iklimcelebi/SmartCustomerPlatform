namespace SmartCustomerPlatform.Persistence.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public int RetryCount { get; set; }

    public string? Error { get; set; }
}
