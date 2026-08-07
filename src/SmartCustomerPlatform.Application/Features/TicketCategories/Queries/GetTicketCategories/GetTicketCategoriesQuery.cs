using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategories;

public record GetTicketCategoriesQuery
    : IRequest<IReadOnlyList<TicketCategory>>;
