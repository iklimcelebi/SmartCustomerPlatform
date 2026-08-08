using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Entities;

public class Package : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal MonthlyFee { get; set; }

    public int TotalQuota { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Subscription> Subscriptions { get; set; }
        = new List<Subscription>();
}
