using Application.IGenericRepository;
using Domain.Entities.Enum;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQContract.Generic.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GenericRepository
{
    public class QueueRepository : IQueueRepository
    {
        private readonly IBus _bus;
        private readonly ILogger<QueueRepository> _logger;
        public QueueRepository(IBus bus, ILogger<QueueRepository> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task EnqueueAddAsync<T>(T entity) where T : class
        {
            _logger.LogInformation($"[Queue] Enqueue Add: {typeof(T).Name}");
            await PublishAsync(entity, QueueActionType.Add);
        }

        public async Task EnqueueUpdateAsync<T>(T entity) where T : class
        {
            _logger.LogInformation($"[Queue] Enqueue Update: {typeof(T).Name}");
            await PublishAsync(entity, QueueActionType.Update);
        }

        public async Task EnqueueDeleteAsync<T>(T entity) where T : class
        {
            _logger.LogInformation($"[Queue] Enqueue Delete: {typeof(T).Name}");
            await PublishAsync(entity, QueueActionType.Delete);
        }

        public async Task EnqueueRangeAsync<T>(IEnumerable<T> entities, QueueActionType actionType) where T : class
        {
            try
            {
                _logger.LogInformation($"[Queue] Enqueue Range ({actionType}): {typeof(T).Name} (Count: {entities.Count()})");
                var jsonPayload = JsonConvert.SerializeObject(entities, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                Console.WriteLine($"[QUEUE_LOG] Action: {actionType}, Entity: {typeof(T).Name}");
                Console.WriteLine($"[QUEUE_LOG] Payload: {jsonPayload}");

                var msg = new GenericQueueMessage
                {
                    ActionType = actionType,
                    EntityType = typeof(T).AssemblyQualifiedName!,
                    PayloadJson = jsonPayload
                };

                await _bus.Publish(msg);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError($"EnqueueRangeAsync {typeof(T).Name} Error: {errorMsg}");
                Console.WriteLine($"[QUEUE_ERROR] {errorMsg}");
                throw;
            }
        }

        private async Task PublishAsync<T>(T entity, QueueActionType actionType) where T : class
        {
            try
            {
                var jsonPayload = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                Console.WriteLine($"[QUEUE_LOG] Action: {actionType}, Entity: {typeof(T).Name}");
                Console.WriteLine($"[QUEUE_LOG] Payload: {jsonPayload}");

                var msg = new GenericQueueMessage
                {
                    ActionType = actionType,
                    EntityType = typeof(T).AssemblyQualifiedName!,
                    PayloadJson = jsonPayload
                };

                await _bus.Publish(msg);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError($"PublishAsync {typeof(T).Name} Error: {errorMsg}");
                Console.WriteLine($"[QUEUE_ERROR] {errorMsg}");
                throw;
            }
        }
    }
}
