using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Persistence.Contexts;

public class SmartCustomerPlatformDbContext : DbContext //inheritance: inherits from DbContext class provided by Entity Framework Core
{
    public SmartCustomerPlatformDbContext(
        DbContextOptions<SmartCustomerPlatformDbContext> options)// <> generic type
        : base(options) //calls the constructor of DBContext 
    {
    }

    public DbSet<Customer> Customers => Set<Customer>(); //creates a DBtable named Customer, also creates getter and setter

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartCustomerPlatformDbContext).Assembly); // applies all entity configurations defined in the assembly where SmartCustomerPlatformDbContext is located. 

    }
}