namespace SmartCustomerPlatform.Persistence.Outbox;

public class ProjectionCheckpoint
{
    public Guid Id { get; set; }

    public string ProjectionName { get; set; } = null!;

    public DateTime LastProcessedOccurredOn { get; set; }

    public Guid LastProcessedMessageId { get; set; }
}
