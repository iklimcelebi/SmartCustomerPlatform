using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaignById;

public record GetCampaignByIdQuery(Guid Id) : IRequest<Campaign?>;
