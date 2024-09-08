using System.Linq.Expressions;

namespace ToursAndTravelsManagement.Repositories.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string includeProperties = null);
    Task<T> GetByIdAsync(int id, string includeProperties = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}