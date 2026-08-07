using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(
    Guid Id,
    string Code,
    string Name,
    string Description,
    DepartmentStatus Status
) : IRequest<bool>;
