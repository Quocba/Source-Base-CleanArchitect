using Application.IGenericRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.GenericRepository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
    public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _dbSet.AddRangeAsync(entities);
    public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
    {
        return await include(_dbSet).Where(predicate).ToListAsync();
    }
    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();
    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        => await include(_dbSet).Where(predicate).ToListAsync();
        
    public async Task<TEntity?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        => await include(_dbSet).Where(predicate).ToListAsync();

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.SingleOrDefaultAsync(predicate);
    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> include, bool disableTracking = false)
    {
        IQueryable<TEntity> query = _dbSet;
        if (disableTracking) query = query.AsNoTracking();
        if (include != null) query = include(query);
        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> include, bool disableTracking = false)
    {
        IQueryable<TEntity> query = _dbSet;
        if (disableTracking) query = query.AsNoTracking();
        if (include != null) query = include(query);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = false)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        if (disableTracking) query = query.AsNoTracking();
        return await query.Select(selector).SingleOrDefaultAsync();
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool disableTracking = false)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        if (disableTracking) query = query.AsNoTracking();
        if (orderBy != null) query = orderBy(query);
        return await query.Select(selector).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = false)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        if (disableTracking) query = query.AsNoTracking();
        return await query.Select(selector).ToListAsync();
    }

    public void Update(TEntity entity) => _dbSet.Update(entity);
    public void Remove(TEntity entity) => _dbSet.Remove(entity);

    public void DeleteHook(object ID, TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var type = entity.GetType();
        var isDeletedProp = type.GetProperty("IsDeleted");
        if (isDeletedProp == null)
            throw new InvalidOperationException($"Entity type '{type.Name}' must declare a boolean 'IsDeleted' property for soft-delete/restore.");

        var propType = Nullable.GetUnderlyingType(isDeletedProp.PropertyType) ?? isDeletedProp.PropertyType;
        if (propType != typeof(bool))
            throw new InvalidOperationException($"Property 'IsDeleted' on '{type.Name}' must be of type 'bool' or 'bool?'.");

        var currentValue = isDeletedProp.GetValue(entity) as bool?;
        if (currentValue == true)
            isDeletedProp.SetValue(entity, false); // Restore
        else
            isDeletedProp.SetValue(entity, true);  // Soft-delete

        Update(entity);
    }

    public async Task<decimal> SumAsync(
     Expression<Func<TEntity, bool>> predicate,
     Expression<Func<TEntity, decimal?>> selector)
    {
        return await _dbSet
            .Where(predicate)
            .SumAsync(selector) ?? 0;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}