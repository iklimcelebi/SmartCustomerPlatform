namespace SmartCustomerPlatform.Persistence.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid AggregateId { get; set; }

    public string AggregateType { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public string? Metadata { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public int RetryCount { get; set; }

    public DateTime? NextRetryAtUtc { get; set; }

    public OutboxStatus Status { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}