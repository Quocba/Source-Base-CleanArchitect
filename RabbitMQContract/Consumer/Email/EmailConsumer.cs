using EmailService.DTO;
using EmailService.Interface;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RabbitMQContract.Consumer.Email
{
    public class EmailConsumer(IEmailSender _emailSender, ILogger<EmailConsumer> _logger) : IConsumer<EmailMessage>
    {
        public async Task Consume(ConsumeContext<EmailMessage> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Processing email to: {To} | Subject: {Subject}", message.To, message.Subject);

                var request = new EmailRequest<string>
                {
                    To = message.To,
                    Subject = message.Subject,
                    Body = message.Body
                };

                await _emailSender.SendEmailAsync(request);
                _logger.LogInformation("Email sent successfully to: {To}", message.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {To}", context.Message?.To);
                throw;
            }
        }
    }
}
