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

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.City)
                    .HasMaxLength(100);

                address.Property(a => a.District)
                    .HasMaxLength(100);

                address.Property(a => a.Street)
                    .HasMaxLength(200);

                address.Property(a => a.PostalCode)
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasMaxLength(100);
            });



            builder.ToTable("Customers");
    }
}