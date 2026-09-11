using System;

namespace RabbitMQContract.Payload.Product
{
    public class ProductCreatedMessage
    {
        public Guid ProductId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
