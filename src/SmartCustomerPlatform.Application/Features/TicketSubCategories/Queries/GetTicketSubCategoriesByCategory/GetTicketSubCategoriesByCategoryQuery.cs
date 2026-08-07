using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoriesByCategory;

public record GetTicketSubCategoriesByCategoryQuery(Guid CategoryId)
    : IRequest<List<TicketSubCategory>>;
