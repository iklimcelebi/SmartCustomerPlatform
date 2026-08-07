using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategories;

public class GetTicketCategoriesQueryHandler
    : IRequestHandler<GetTicketCategoriesQuery, IReadOnlyList<TicketCategory>>
{
    private readonly ITicketCategoryRepository _repository;

    public GetTicketCategoriesQueryHandler(
        ITicketCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TicketCategory>> Handle(
        GetTicketCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
