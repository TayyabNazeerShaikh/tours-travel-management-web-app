using ToursAndTravelsManagement.Common;

namespace ToursAndTravelsManagement.Repositories.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string includeProperties = null);
    Task<T> GetByIdAsync(int id, string includeProperties = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<PaginatedList<T>> GetPaginatedAsync(int pageNumber, int pageSize, string includeProperties = null);
}