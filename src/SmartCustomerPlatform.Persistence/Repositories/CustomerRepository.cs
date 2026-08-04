using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class CustomerRepository
    : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(SmartCustomerPlatformDbContext context)
        : base(context)// its used for constructor to work with base class constructor.
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Email == email);
    }
}