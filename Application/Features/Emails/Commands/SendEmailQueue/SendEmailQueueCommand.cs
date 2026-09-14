using Domain.Payload.Base;
using MediatR;

namespace Application.Features.Emails.Commands.SendEmailQueue
{
    public class SendEmailQueueCommand : IRequest<ApiResponse<string>>
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
