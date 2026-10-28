using Domain.Payload.Base;
using Application.Common.Share.Common;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQContract.Consumer.Email;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Emails.Commands.SendEmailQueue
{
    /*
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
                        Message = "To vÃ  Subject khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng."
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
                    Message = "YÃªu cáº§u gá»­i email Ä‘Ã£ Ä‘Æ°á»£c Ä‘Æ°a vÃ o hÃ ng Ä‘á»£i xá»­ lÃ½.",
                    Data = request.To
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lá»—i khi gá»­i email vÃ o hÃ ng Ä‘á»£i: {Message}", ex.Message);
                throw;
            }
        }
    }
    */
}
