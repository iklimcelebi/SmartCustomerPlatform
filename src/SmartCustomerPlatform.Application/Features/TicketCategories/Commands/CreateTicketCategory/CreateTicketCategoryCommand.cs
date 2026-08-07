using MediatR;

namespace SmartCustomerPlatform.Application.Features.TicketCategories.Commands.CreateTicketCategory;

public record CreateTicketCategoryCommand(
    string Code,
    string Name,
    string Description,
    bool IsActive
) : IRequest<Guid>;
