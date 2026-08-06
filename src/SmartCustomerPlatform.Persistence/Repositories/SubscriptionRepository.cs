using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SmartCustomerPlatformDbContext _context;

    public SubscriptionRepository(
        SmartCustomerPlatformDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default)
    {
        await _context
            .Set<Subscription>()
            .AddAsync(subscription, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}