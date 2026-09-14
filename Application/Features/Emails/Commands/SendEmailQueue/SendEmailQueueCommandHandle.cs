using Domain.Payload.Base;
using Domain.Share.Common;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQContract.Consumer.Email;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Emails.Commands.SendEmailQueue
{
    public class SendEmailQueueCommandHandle(IPublishEndpoint _publishEndpoint,
                                             ILogger<SendEmailQueueCommandHandle> _logger)
        : IRequestHandler<SendEmailQueueCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(SendEmailQueueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Subject))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "To và Subject không được để trống."
                    };
                }

                await _publishEndpoint.Publish(new EmailMessage
                {
                    To = request.To.Trim(),
                    Subject = request.Subject.Trim(),
                    Body = request.Body
                }, cancellationToken);

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Accepted,
                    Message = "Yêu cầu gửi email đã được đưa vào hàng đợi xử lý.",
                    Data = request.To
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email vào hàng đợi: {Message}", ex.Message);
                throw;
            }
        }
    }
}
