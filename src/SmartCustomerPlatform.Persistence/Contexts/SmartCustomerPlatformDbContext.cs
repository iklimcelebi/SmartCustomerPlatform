using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Persistence.Contexts;

public class SmartCustomerPlatformDbContext : DbContext
{
    private readonly IMediator _mediator;

    public SmartCustomerPlatformDbContext(
        DbContextOptions<SmartCustomerPlatformDbContext> options,
        IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();

    public DbSet<TicketSubCategory> TicketSubCategories => Set<TicketSubCategory>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Package> Packages => Set<Package>();

    public DbSet<Campaign> Campaigns => Set<Campaign>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartCustomerPlatformDbContext).Assembly);
    }
}

