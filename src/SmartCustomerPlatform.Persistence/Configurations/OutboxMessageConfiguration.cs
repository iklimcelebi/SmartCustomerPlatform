using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCustomerPlatform.Persistence.Outbox;

namespace SmartCustomerPlatform.Persistence.Configurations;

public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.AggregateId)
            .IsRequired();

        builder.Property(x => x.AggregateType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.Metadata);

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedAtUtc);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.NextRetryAtUtc);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ErrorMessage);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.EventId)
            .IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.NextRetryAtUtc);

        builder.HasIndex(x => x.OccurredAtUtc);
    }
}