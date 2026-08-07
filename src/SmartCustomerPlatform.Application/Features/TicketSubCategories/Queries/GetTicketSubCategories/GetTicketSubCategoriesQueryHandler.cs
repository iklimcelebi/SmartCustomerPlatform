using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategories;

public class GetTicketSubCategoriesQueryHandler
    : IRequestHandler<GetTicketSubCategoriesQuery, List<TicketSubCategory>>
{
    private readonly ITicketSubCategoryRepository _repository;

    public GetTicketSubCategoriesQueryHandler(
        ITicketSubCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TicketSubCategory>> Handle(
        GetTicketSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync();

        return result.ToList();
    }
}
