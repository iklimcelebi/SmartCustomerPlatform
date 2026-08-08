using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.MonthlyFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TotalQuota)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Subscriptions)
            .WithOne()
            .HasForeignKey("PackageId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable("Packages");
    }
}
