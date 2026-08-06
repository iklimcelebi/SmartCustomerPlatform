using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SmartCustomerPlatformDbContext _context;

    public SubscriptionRepository(SmartCustomerPlatformDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default)
    {
        await _context.Subscriptions.AddAsync(
            subscription,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Subscription?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Subscription>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default)
    {
        _context.Subscriptions.Update(subscription);

        await _context.SaveChangesAsync(cancellationToken);
    }
}