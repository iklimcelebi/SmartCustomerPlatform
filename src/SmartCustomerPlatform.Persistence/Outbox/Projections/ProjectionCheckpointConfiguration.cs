using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartCustomerPlatform.Persistence.Projections;

public class ProjectionCheckpointConfiguration
    : IEntityTypeConfiguration<ProjectionCheckpoint>
{
    public void Configure(
        EntityTypeBuilder<ProjectionCheckpoint> builder)
    {
        builder.ToTable("ProjectionCheckpoints");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProjectionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.ProjectionName)
            .IsUnique();

        builder.Property(x => x.CommitPosition)
            .IsRequired();

        builder.Property(x => x.PreparePosition)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);
    }
}