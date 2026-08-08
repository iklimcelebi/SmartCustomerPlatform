using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class PackageRepository : GenericRepository<Package>, IPackageRepository
{
    public PackageRepository(SmartCustomerPlatformDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Package>> GetActivePackagesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(x => x.IsActive).ToListAsync(cancellationToken);
    }
}
