using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class TicketSubCategoryRepository
    : GenericRepository<TicketSubCategory>,
      ITicketSubCategoryRepository
{
    private readonly SmartCustomerPlatformDbContext _dbContext;

    public TicketSubCategoryRepository(
        SmartCustomerPlatformDbContext context)
        : base(context)
    {
        _dbContext = context;
    }

    public async Task<IReadOnlyList<TicketSubCategory>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TicketSubCategory>()
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }
}
