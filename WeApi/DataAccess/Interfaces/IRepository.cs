using System.Linq.Expressions;

namespace DataAccess.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, bool isTracking = true);
    Task<T?> GetById(int id);
    Task Add(T entity);
    Task AddRange(IEnumerable<T> entities);
    void Delete(T entity);
    void Update(T entity);
}
