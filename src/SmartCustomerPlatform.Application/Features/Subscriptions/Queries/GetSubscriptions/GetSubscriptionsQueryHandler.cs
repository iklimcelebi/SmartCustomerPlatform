using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptions;

public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, IReadOnlyList<Subscription>>
{
    private readonly IGenericRepository<Subscription> _repository;

    public GetSubscriptionsQueryHandler(IGenericRepository<Subscription> repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Subscription>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
