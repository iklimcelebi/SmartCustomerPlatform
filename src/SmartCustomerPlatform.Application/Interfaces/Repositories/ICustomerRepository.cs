using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>// inherites from IGenericRepository interface
//thanks to this IcustomerRepository otomatically has all the methods of IGenericRepository.
{
    Task<Customer?> GetByEmailAsync(string email);
}