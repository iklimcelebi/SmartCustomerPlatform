using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(
    string Code,
    string Name,
    string Description,
    DepartmentStatus Status
) : IRequest<Guid>;


