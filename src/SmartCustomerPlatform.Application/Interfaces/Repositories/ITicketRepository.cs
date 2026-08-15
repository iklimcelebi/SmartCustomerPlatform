using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<Ticket?> GetByTicketNumberAsync(string ticketNumber);

    Task<Ticket?> GetByIdWithDetailsAsync(Guid id);

    Task<IReadOnlyList<Ticket>> GetByCustomerIdAsync(Guid customerId);

    Task<IReadOnlyList<Ticket>> GetAllWithDetailsAsync();
}