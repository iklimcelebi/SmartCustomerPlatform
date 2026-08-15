using MediatR;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid Id) : IRequest<bool>;