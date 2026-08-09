using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Outbox;

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

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();

    public DbSet<TicketSubCategory> TicketSubCategories => Set<TicketSubCategory>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent),
                OccurredOn = DateTime.UtcNow,
                RetryCount = 0
            };

            OutboxMessages.Add(outboxMessage);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return result;
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartCustomerPlatformDbContext).Assembly);
    }
}






