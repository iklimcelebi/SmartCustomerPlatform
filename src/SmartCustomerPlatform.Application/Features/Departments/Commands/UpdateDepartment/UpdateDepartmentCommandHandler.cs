using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler
    : IRequestHandler<UpdateDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departmentRepository;

    public UpdateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<bool> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository
            .GetByIdAsync(request.Id);

        if (department is null)
            return false;

        department.Code = request.Code;
        department.Name = request.Name;
        department.Description = request.Description;
        department.Status = request.Status;

        _departmentRepository.Update(department);
        await _departmentRepository.SaveChangesAsync();

        return true;
    }
}
