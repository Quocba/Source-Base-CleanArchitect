using Domain.Enums;

namespace RabbitMQContract.Generic
{
    public class GenericQueueMessage
    {
        public QueueActionType ActionType { get; set; }
        public string EntityType { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
    }
}
