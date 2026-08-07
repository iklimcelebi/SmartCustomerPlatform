using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoriesByCategory;

public class GetTicketSubCategoriesByCategoryQueryHandler
    : IRequestHandler<
        GetTicketSubCategoriesByCategoryQuery,
        List<TicketSubCategory>>
{
    private readonly ITicketSubCategoryRepository _repository;

    public GetTicketSubCategoriesByCategoryQueryHandler(
        ITicketSubCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TicketSubCategory>> Handle(
        GetTicketSubCategoriesByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var subCategories = await _repository.GetAllAsync();

        return subCategories
            .Where(x => x.CategoryId == request.CategoryId)
            .ToList();
    }
}
