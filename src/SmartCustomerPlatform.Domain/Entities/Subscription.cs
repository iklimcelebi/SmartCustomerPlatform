using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Subscription
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    public Guid PackageId { get; private set; }

    public decimal MonthlyPrice { get; private set; }

    public Guid? CampaignId { get; private set; }

    public decimal? DiscountedPrice { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public bool IsActive { get; private set; }

    private Subscription()
    {
    }

    public Subscription(
        string name,
        Guid customerId,
        Guid packageId,
        decimal monthlyPrice,
        DateTime startDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        CustomerId = customerId;
        PackageId = packageId;
        MonthlyPrice = monthlyPrice;
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
public void ChangePackage(Guid packageId, decimal monthlyPrice)
{
    PackageId = packageId;
    MonthlyPrice = monthlyPrice;
}
}