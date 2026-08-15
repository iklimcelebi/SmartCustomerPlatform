using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler
    : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departmentRepository;

    public DeleteDepartmentCommandHandler(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<bool> Handle(
        DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository
            .GetByIdAsync(request.Id);

        if (department is null)
            return false;

        _departmentRepository.Delete(department);

        await _departmentRepository.SaveChangesAsync();

        return true;
    }
}