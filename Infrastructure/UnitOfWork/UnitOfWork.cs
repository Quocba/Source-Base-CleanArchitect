using Application.IGenericRepository;
using Application.IUnitOfWork;
using Infrastructure.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    public DbContext Context => _context;
    private readonly TContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        if (_repositories.TryGetValue(type, out var repo))
            return (IGenericRepository<TEntity>)repo;

        var repository = new GenericRepository<TEntity>(_context);
        _repositories[type] = repository;
        return repository;
    }

    public int Commit()
    {
        TrackChanges();
        return _context.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
        TrackChanges();
        return await _context.SaveChangesAsync();
    }

    private void TrackChanges()
    {
        var errors = _context.ChangeTracker.Entries<IValidatableObject>()
            .SelectMany(e => e.Entity.Validate(null))
            .Where(e => e != ValidationResult.Success)
            .ToArray();

        if (errors.Any())
        {
            var msg = string.Join(Environment.NewLine,
                errors.Select(e => $"Properties {string.Join(",", e.MemberNames)} Error: {e.ErrorMessage}"));
            throw new Exception(msg);
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context?.Dispose();
    }
}
