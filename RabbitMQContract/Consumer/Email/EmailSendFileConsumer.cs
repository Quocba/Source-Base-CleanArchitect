using EmailService.DTO;
using EmailService.Interface;
using MassTransit;
using RabbitMQContract.Payload.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQContract.Consumer
{
    public class EmailSendFileConsumer : IConsumer<EmailSendFileMessage>
    {
        private readonly IEmailSender _emailSender;
        public EmailSendFileConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<EmailSendFileMessage> context)
        {
            try
            {
                var message = context.Message;

                var request = new EmailRequest<string>
                {
                    To = message.To,
                    Subject = message.Subject,
                    Body = message.Body
                };

                if (message.FileBytes != null && message.FileName != null)
                {
                    using var stream = new MemoryStream(message.FileBytes);
                    await _emailSender.SendEmailWithAttachmentAsync(request, stream, message.FileName);
                }
                else
                {
                    await _emailSender.SendEmailAsync(request);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
