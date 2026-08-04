namespace SmartCustomerPlatform.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class // where T : class is a generic constraint. this means that interface can only use reference types (classes).
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}
