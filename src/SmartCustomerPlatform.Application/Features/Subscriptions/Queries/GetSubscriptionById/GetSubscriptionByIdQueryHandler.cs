using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, Subscription?>
{
    private readonly IGenericRepository<Subscription> _repository;

    public GetSubscriptionByIdQueryHandler(IGenericRepository<Subscription> repository)
    {
        _repository = repository;
    }

    public async Task<Subscription?> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
