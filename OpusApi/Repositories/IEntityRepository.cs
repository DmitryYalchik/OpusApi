using System.Linq.Expressions;
using OpusApi.DbModels;

namespace OpusApi.Repositories;

public interface IEntityRepository<T> where T : IdentityModel
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>?> GetAllAsync();
    Task<IEnumerable<T>?> FindAsync(Expression<Func<T, bool>> predicate);
    
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    
    Task Update(T entity);
    
    Task Delete(Guid id);
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}