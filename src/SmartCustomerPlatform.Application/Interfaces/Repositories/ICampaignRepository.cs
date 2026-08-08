using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface ICampaignRepository : IGenericRepository<Campaign>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Campaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default);
}
