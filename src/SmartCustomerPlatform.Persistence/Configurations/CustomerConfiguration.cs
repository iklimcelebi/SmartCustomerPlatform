using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer> //Interface implementation
{
    public void Configure(EntityTypeBuilder<Customer> builder)// configure method
    {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);   

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.TCKNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(x => x.Status)
                .IsRequired();
            builder.OwnsOne(x => x.Address);

            builder.ToTable("Customers");
    }
}