namespace SmartCustomerPlatform.Worker.Projections;

public class SubscriptionCreatedEvent
{
    public Guid EventId { get; set; }

    public Guid AggregateId { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string CorrelationId { get; set; } = string.Empty;

    public string? CausationId { get; set; }

    public string? PerformedBy { get; set; }

    public Guid CustomerId { get; set; }

    public Guid PackageId { get; set; }

    public Guid? CampaignId { get; set; }

    public decimal MonthlyPrice { get; set; }

    public decimal DiscountedPrice { get; set; }

    public DateTime StartDate { get; set; }
}