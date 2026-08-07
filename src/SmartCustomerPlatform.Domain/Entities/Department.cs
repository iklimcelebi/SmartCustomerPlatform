using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Department : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DepartmentStatus Status { get; set; } = DepartmentStatus.Active;
}
