using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubscriptionNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.PackageName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MonthlyFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountedPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.UsedQuota)
            .IsRequired();

        builder.Property(x => x.RemainingQuota)
            .IsRequired();

        builder.Property(x => x.TotalQuota)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate);

        builder.Property(x => x.IsAutoRenew)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Package)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campaign)
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable("Subscriptions");
    }
}

