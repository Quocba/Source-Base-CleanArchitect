using Domain.Entities.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IGenericRepository
{
    public interface IQueueRepository
    {
        Task EnqueueAddAsync<T>(T entity) where T : class;
        Task EnqueueUpdateAsync<T>(T entity) where T : class;
        Task EnqueueDeleteAsync<T>(T entity) where T : class;
        Task EnqueueRangeAsync<T>(IEnumerable<T> entities, QueueActionType actionType) where T : class;
    }
}