using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Guid> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Status = request.Status
        };

        await _departmentRepository.AddAsync(department);
        await _departmentRepository.SaveChangesAsync();

        return department.Id;
    }
}