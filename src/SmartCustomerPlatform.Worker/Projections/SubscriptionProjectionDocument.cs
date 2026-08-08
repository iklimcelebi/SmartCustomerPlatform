namespace SmartCustomerPlatform.Worker.Projections;

public class SubscriptionProjectionDocument
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid PackageId { get; set; }

    public decimal MonthlyPrice { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid? CampaignId { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public bool IsActive { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; }
}