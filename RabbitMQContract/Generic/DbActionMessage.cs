using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQContract.Generic
{
    public class DbActionMessage
    {
        public string Action { get; set; } 
        public string EntityType { get; set; }
        public string Payload { get; set; }
    }
}
