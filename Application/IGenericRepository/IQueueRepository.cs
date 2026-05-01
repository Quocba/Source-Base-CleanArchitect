using Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GenericRepository
{
    public interface IQueueRepository
    {
        /// <summary>
        /// Gửi 1 entity vào queue để thêm mới (Add).
        /// </summary>
        Task EnqueueAddAsync<T>(T entity) where T : class;

        /// <summary>
        /// Gửi 1 entity vào queue để cập nhật (Update).
        /// </summary>
        Task EnqueueUpdateAsync<T>(T entity) where T : class;

        /// <summary>
        /// Gửi 1 entity vào queue để xóa (Delete).
        /// </summary>
        Task EnqueueDeleteAsync<T>(T entity) where T : class;

        /// <summary>
        /// Gửi danh sách entity vào queue để thêm hàng loạt.
        /// </summary>
        Task EnqueueRangeAsync<T>(IEnumerable<T> entities, QueueActionType actionType) where T : class;
    }
}