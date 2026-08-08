using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IReadOnlyList<Comment>> GetByTicketIdAsync(Guid ticketId);

    Task<Comment?> GetByIdIncludingDeletedAsync(Guid id);
}
