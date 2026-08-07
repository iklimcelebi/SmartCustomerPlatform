using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class TicketCategoryRepository
    : GenericRepository<TicketCategory>,
      ITicketCategoryRepository
{
    public TicketCategoryRepository(
        SmartCustomerPlatformDbContext context)
        : base(context)
    {
    }
}
