using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Campaigns.Commands.CreateCampaign;

public record CreateCampaignCommand(
    string Code,
    string Name,
    string Description,
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive
) : IRequest<Guid>;
