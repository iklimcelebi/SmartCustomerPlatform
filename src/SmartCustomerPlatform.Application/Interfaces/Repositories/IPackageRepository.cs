using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface IPackageRepository : IGenericRepository<Package>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Package>> GetActivePackagesAsync(CancellationToken cancellationToken = default);
}
