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

    public DbSet<TicketCategory> TicketCategories =>
        Set<TicketCategory>();

    public DbSet<TicketSubCategory> TicketSubCategories =>
        Set<TicketSubCategory>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<OutboxMessage> OutboxMessages =>
        Set<OutboxMessage>();

    public DbSet<ProjectionCheckpoint> ProjectionCheckpoints =>
        Set<ProjectionCheckpoint>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        /*
         * CreatedAt ve UpdatedAt alanlarını
         * tüm BaseEntity nesneleri için otomatik yönet.
         *
         * Yeni kayıt:
         * CreatedAt = şu an
         *
         * Güncellenen kayıt:
         * UpdatedAt = şu an
         */
        var baseEntityEntries =
            ChangeTracker
                .Entries<BaseEntity>()
                .ToList();

        var now = DateTime.UtcNow;

        foreach (var entry in baseEntityEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;

                /*
                 * CreatedAt hiçbir zaman güncellenmesin.
                 */
                entry.Property(
                    nameof(BaseEntity.CreatedAt))
                    .IsModified = false;
            }
        }

        /*
         * Domain event'lerini Outbox'a ekle.
         */
        var domainEvents = baseEntityEntries
            .SelectMany(
                entry => entry.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var eventType =
                domainEvent.GetType().Name;

            var payload =
                JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType());

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                OccurredOn =
                    domainEvent.OccurredOn,
                RetryCount = 0
            };

            OutboxMessages.Add(
                outboxMessage);
        }

        var result =
            await base.SaveChangesAsync(
                cancellationToken);

        /*
         * Event'ler Outbox'a aktarıldıktan sonra
         * entity üzerindeki domain event listesini temizle.
         */
        foreach (var entry in baseEntityEntries)
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
            typeof(SmartCustomerPlatformDbContext)
                .Assembly);
    }
}


