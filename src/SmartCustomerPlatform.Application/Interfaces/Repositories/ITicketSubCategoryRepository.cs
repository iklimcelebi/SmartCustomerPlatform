using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface ITicketSubCategoryRepository
    : IGenericRepository<TicketSubCategory>
{
    Task<IReadOnlyList<TicketSubCategory>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
