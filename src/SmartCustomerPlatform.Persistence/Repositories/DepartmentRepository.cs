using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class DepartmentRepository
    : GenericRepository<Department>,
      IDepartmentRepository
{
    public DepartmentRepository(
        SmartCustomerPlatformDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }
}
