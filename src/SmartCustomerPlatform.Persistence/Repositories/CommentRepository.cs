using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(SmartCustomerPlatformDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Comment>> GetByTicketIdAsync(
        Guid ticketId)
    {
        return await _context.Comments
            .Where(x => x.TicketId == ticketId && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdIncludingDeletedAsync(
        Guid id)
    {
        return await _context.Comments
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
