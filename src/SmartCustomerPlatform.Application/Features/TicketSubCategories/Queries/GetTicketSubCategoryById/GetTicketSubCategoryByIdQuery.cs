using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoryById;

public record GetTicketSubCategoryByIdQuery(Guid Id)
    : IRequest<TicketSubCategory?>;
