using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategories;

public record GetTicketSubCategoriesQuery
    : IRequest<List<TicketSubCategory>>;
