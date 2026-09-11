using MassTransit;
using Microsoft.Extensions.Logging;
using RabbitMQContract.Payload.Product;
using RedisService.IService;
using System.Threading.Tasks;

namespace RabbitMQContract.Consumer.Product
{
    public class ProductCreatedConsumer(ILogger<ProductCreatedConsumer> _logger, IRedisService _redisService) 
        : IConsumer<ProductCreatedMessage>
    {
        public async Task Consume(ConsumeContext<ProductCreatedMessage> context)
        {
            var msg = context.Message;
            _logger.LogInformation("🧑‍💻 [RabbitMQ Consumer] Nhận message sản phẩm mới: {Code} - {Name} (Giá: {Price:N0} VNĐ)", 
                msg.Code, msg.Name, msg.Price);

            // Invalidate cache Redis cho danh sách lookup sản phẩm để FE tự động nhận dữ liệu mới nhất
            await _redisService.RemoveAsync("products_lookup");
            _logger.LogInformation("🧑‍💻 [RabbitMQ Consumer] Đã xóa cache products_lookup thành công");
        }
    }
}
