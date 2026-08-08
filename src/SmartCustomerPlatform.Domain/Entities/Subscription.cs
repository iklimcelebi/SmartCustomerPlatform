using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Domain.Events;

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

        RaiseDomainEvent(
            new SubscriptionCreatedDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null)
            {
                CustomerId = CustomerId,
                PackageId = PackageId,
                CampaignId = CampaignId,
                MonthlyPrice = MonthlyPrice,
                DiscountedPrice = DiscountedPrice ?? MonthlyPrice,
                StartDate = StartDate
            });
    }

    public void Activate()
    {
        IsActive = true;
        Status = SubscriptionStatus.Active;
        EndDate = null;

        RaiseDomainEvent(
            new SubscriptionActivatedDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null));
    }

    public void Freeze()
    {
        IsActive = false;
        Status = SubscriptionStatus.Frozen;

        RaiseDomainEvent(
            new SubscriptionFrozenDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null));
    }

    public void Resume()
    {
        IsActive = true;
        Status = SubscriptionStatus.Active;
        EndDate = null;

        RaiseDomainEvent(
            new SubscriptionUnfrozenDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null));
    }

    public void Cancel()
    {
        IsActive = false;
        Status = SubscriptionStatus.Cancelled;
        EndDate = DateTime.UtcNow;

        RaiseDomainEvent(
            new SubscriptionCancelledDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null));
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

        RaiseDomainEvent(
            new SubscriptionPackageChangedDomainEvent(
                Guid.NewGuid(),
                Id,
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                null,
                null)
            {
                PackageId = PackageId,
                MonthlyPrice = MonthlyPrice
            });
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