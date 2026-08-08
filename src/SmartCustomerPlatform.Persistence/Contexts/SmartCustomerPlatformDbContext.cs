using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Outbox;
using SmartCustomerPlatform.Persistence.Projections;

namespace SmartCustomerPlatform.Persistence.Contexts;

public class SmartCustomerPlatformDbContext : DbContext
{
    public SmartCustomerPlatformDbContext(
        DbContextOptions<SmartCustomerPlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<ProjectionCheckpoint> ProjectionCheckpoints
        => Set<ProjectionCheckpoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartCustomerPlatformDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(x => x.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventId = domainEvent.EventId,
                AggregateId = domainEvent.AggregateId,
                AggregateType = "Subscription",
                EventType = domainEvent.EventType,
                Payload = JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType()),
                Metadata = JsonSerializer.Serialize(new
                {
                    domainEvent.CorrelationId,
                    domainEvent.CausationId,
                    domainEvent.PerformedBy
                }),
                OccurredAtUtc = domainEvent.OccurredAtUtc,
                ProcessedAtUtc = null,
                RetryCount = 0,
                NextRetryAtUtc = null,
                Status = OutboxStatus.Pending,
                ErrorMessage = null,
                CreatedAtUtc = DateTime.UtcNow
            };

            OutboxMessages.Add(outboxMessage);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return result;
    }
}