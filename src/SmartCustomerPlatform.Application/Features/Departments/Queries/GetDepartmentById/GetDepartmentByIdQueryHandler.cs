using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQueryHandler
    : IRequestHandler<GetDepartmentByIdQuery, Department?>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentByIdQueryHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Department?> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _departmentRepository.GetByIdAsync(request.Id);
    }
}
