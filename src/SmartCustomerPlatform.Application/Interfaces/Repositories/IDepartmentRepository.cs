using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}
