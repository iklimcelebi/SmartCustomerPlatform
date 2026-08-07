using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategoryById;

public class GetTicketCategoryByIdQueryHandler
    : IRequestHandler<GetTicketCategoryByIdQuery, TicketCategory?>
{
    private readonly ITicketCategoryRepository _repository;

    public GetTicketCategoryByIdQueryHandler(
        ITicketCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketCategory?> Handle(
        GetTicketCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
