using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    Task<int> CommitAsync();
    int Commit();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    DbContext Context { get; }
}
