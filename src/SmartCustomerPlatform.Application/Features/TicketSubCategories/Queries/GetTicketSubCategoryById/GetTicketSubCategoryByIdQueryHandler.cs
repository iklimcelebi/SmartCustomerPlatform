using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoryById;

public class GetTicketSubCategoryByIdQueryHandler
    : IRequestHandler<GetTicketSubCategoryByIdQuery, TicketSubCategory?>
{
    private readonly ITicketSubCategoryRepository _repository;

    public GetTicketSubCategoryByIdQueryHandler(
        ITicketSubCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketSubCategory?> Handle(
        GetTicketSubCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
