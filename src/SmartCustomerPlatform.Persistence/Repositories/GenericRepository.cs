using Microsoft.EntityFrameworkCore;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Persistence.Contexts;

namespace SmartCustomerPlatform.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly SmartCustomerPlatformDbContext _context; // protected means this field can only be used in this class or in the classes who inherits this class.
    // readonly means this field can only be assigned in the constructor and cannot be modified afterwards.
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(SmartCustomerPlatformDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // thanks to generic we work for every entity with one code.
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync(); // await provides work in other subjects while waiting for the result.
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }


    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
