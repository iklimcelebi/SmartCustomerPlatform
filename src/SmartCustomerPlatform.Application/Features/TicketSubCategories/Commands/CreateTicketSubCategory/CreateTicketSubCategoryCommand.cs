using MediatR;

namespace SmartCustomerPlatform.Application.Features.TicketSubCategories.Commands.CreateTicketSubCategory;

public record CreateTicketSubCategoryCommand(
    string Code,
    string Name,
    string Description,
    bool IsActive,
    Guid CategoryId
) : IRequest<Guid>;
