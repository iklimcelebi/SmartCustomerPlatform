using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Entities;

public class TicketSubCategory : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }

    public TicketCategory Category { get; set; } = null!;
}
