using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IRequest<IReadOnlyList<Department>>;