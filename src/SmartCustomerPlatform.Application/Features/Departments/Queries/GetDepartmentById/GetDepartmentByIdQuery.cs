using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartmentById;

public record GetDepartmentByIdQuery(Guid Id)
    : IRequest<Department?>;
