using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler
    : IRequestHandler<GetDepartmentsQuery, IReadOnlyList<Department>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyList<Department>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _departmentRepository.GetAllAsync();
    }
}