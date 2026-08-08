using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(SmartCustomerPlatformDbContext context)
        : base(context)
    {
    }

    public async Task<Ticket?> GetByTicketNumberAsync(
        string ticketNumber)
    {
        return await _context.Tickets
            .Include(x => x.Customer)
            .Include(x => x.Department)
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .FirstOrDefaultAsync(x => x.TicketNumber == ticketNumber);
    }

    public async Task<IReadOnlyList<Ticket>> GetByCustomerIdAsync(
        Guid customerId)
    {
        return await _context.Tickets
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Department)
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .ToListAsync();
    }
}
