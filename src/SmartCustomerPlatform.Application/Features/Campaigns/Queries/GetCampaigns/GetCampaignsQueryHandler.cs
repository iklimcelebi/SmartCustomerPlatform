using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaigns;

public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, IReadOnlyList<Campaign>>
{
    private readonly ICampaignRepository _repository;

    public GetCampaignsQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Campaign>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
