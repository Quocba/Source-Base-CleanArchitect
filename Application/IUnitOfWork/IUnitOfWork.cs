using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    Task<int> CommitAsync();
    int Commit();

    DbContext Context { get; }
}
