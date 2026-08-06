using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Common.Interfaces;

public interface ISubscriptionRepository
{
    Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);

    Task<Subscription?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Subscription>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);
}