using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Persistence;

public class Repository<T>(DbContext context) : IRepository<T> where T : class
{
    private readonly DbContext _context = context;
    internal DbSet<T> dbSet = context.Set<T>();

    public async Task Add(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task AddRange(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, bool isTracking = true)
    {
        IQueryable<T> query = dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);
        }

        if (orderBy != null)
            query = orderBy(query);

        if (!isTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await dbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        dbSet.Update(entity);
    }
}
