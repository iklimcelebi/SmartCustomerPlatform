using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategoryById;

public record GetTicketCategoryByIdQuery(Guid Id)
    : IRequest<TicketCategory?>;
