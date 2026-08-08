using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Campaigns.Queries.GetCampaignById;

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Campaign?>
{
    private readonly ICampaignRepository _repository;

    public GetCampaignByIdQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Campaign?> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
