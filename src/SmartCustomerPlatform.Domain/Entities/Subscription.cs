using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Subscription : BaseEntity
{
    public string SubscriptionNumber { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }

    public int UsedQuota { get; set; }
    public int RemainingQuota { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsAutoRenew { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;
}
