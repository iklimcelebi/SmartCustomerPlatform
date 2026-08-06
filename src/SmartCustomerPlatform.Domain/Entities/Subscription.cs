using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid CustomerId { get; private set; }

    public Guid PackageId { get; private set; }

    public Guid? CampaignId { get; private set; }

    public decimal MonthlyPrice { get; private set; }

    public decimal? DiscountedPrice { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public bool IsActive { get; private set; }

    public Customer Customer { get; private set; } = null!;

    private Subscription()
    {
    }

    public Subscription(
        Guid customerId,
        Guid packageId,
        decimal monthlyPrice,
        DateTime startDate,
        Guid? campaignId = null,
        decimal? discountedPrice = null)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        PackageId = packageId;
        CampaignId = campaignId;
        MonthlyPrice = monthlyPrice;
        DiscountedPrice = discountedPrice;
        StartDate = startDate;

        IsActive = true;
        Status = SubscriptionStatus.PendingActivation;
    }

    public void Activate()
    {
        IsActive = true;
        Status = SubscriptionStatus.Active;
        EndDate = null;
    }

    public void Freeze()
    {
        IsActive = false;
        Status = SubscriptionStatus.Frozen;
    }

    public void Resume()
    {
        IsActive = true;
        Status = SubscriptionStatus.Active;
        EndDate = null;
    }

    public void Cancel()
    {
        IsActive = false;
        Status = SubscriptionStatus.Cancelled;
        EndDate = DateTime.UtcNow;
    }

    public void Expire()
    {
        IsActive = false;
        Status = SubscriptionStatus.Expired;
        EndDate = DateTime.UtcNow;
    }

    public void ChangePackage(
        Guid packageId,
        decimal monthlyPrice)
    {
        PackageId = packageId;
        MonthlyPrice = monthlyPrice;
    }

    public void UpdateDetails(
        decimal monthlyPrice,
        DateTime startDate,
        DateTime? endDate,
        Guid? campaignId,
        decimal? discountedPrice,
        bool isActive)
    {
        MonthlyPrice = monthlyPrice;
        StartDate = startDate;
        EndDate = endDate;
        CampaignId = campaignId;
        DiscountedPrice = discountedPrice;
        IsActive = isActive;
    }
}