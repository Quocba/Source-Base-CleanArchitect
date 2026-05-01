using Application.IGenericRepository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Application.IUnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    Task<int> CommitAsync();
    int Commit();

    DbContext Context { get; }
}
