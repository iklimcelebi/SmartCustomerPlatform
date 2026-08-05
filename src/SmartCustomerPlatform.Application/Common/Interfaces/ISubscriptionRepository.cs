using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Common.Interfaces;

public interface ISubscriptionRepository
{
    Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);
}