using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Domain.Events;
namespace SmartCustomerPlatform.Domain.Entities;

public class Subscription : BaseEntity
{
    public string SubscriptionNumber { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public decimal MonthlyFee { get; set; }

    public decimal DiscountedPrice { get; set; }

    public int UsedQuota { get; set; }

    public int RemainingQuota { get; set; }

    public int TotalQuota { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsAutoRenew { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public Guid PackageId { get; set; }

    public Package Package { get; set; } = null!;

    public Guid? CampaignId { get; set; }



    public Campaign? Campaign { get; set; }


    public void AddCreatedEvent()
    {
        AddDomainEvent(
            new SubscriptionCreatedEvent(
                Id,
                CustomerId,
                PackageId,
                CampaignId,
                MonthlyFee,
                DiscountedPrice,
                StartDate
            )
        );
    }

}



