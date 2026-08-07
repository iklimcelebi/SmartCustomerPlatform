using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Seed;

public static class DepartmentSeed
{
    public static async Task SeedAsync(
        SmartCustomerPlatformDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;

        var departments = new List<Department>
        {
            new Department
            {
                Code = "TECH",
                Name = "Technical Support",
                Description = "Technical customer support department",
                Status = DepartmentStatus.Active
            },
            new Department
            {
                Code = "BILLING",
                Name = "Billing Support",
                Description = "Billing and payment support department",
                Status = DepartmentStatus.Active
            }
        };

        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();
    }
}