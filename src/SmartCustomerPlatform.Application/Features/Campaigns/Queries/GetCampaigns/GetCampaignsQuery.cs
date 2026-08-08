using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaigns;

public record GetCampaignsQuery : IRequest<IReadOnlyList<Campaign>>;
