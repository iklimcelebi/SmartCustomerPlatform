using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Entities;

public class TicketCategory : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<TicketSubCategory> SubCategories { get; set; }
        = new List<TicketSubCategory>();
}
