using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Campaign : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

    public decimal DiscountValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
