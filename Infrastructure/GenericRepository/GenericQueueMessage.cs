using Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQContract.Generic.Request
{
    public class GenericQueueMessage
    {
        public QueueActionType ActionType { get; set; }
        public string EntityType { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
    }
}