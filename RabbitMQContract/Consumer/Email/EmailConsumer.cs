using EmailService.DTO;
using EmailService.Interface;
using MassTransit;

namespace RabbitMQContract.Consumer.Email
{
    public class EmailConsumer : IConsumer<EmailMessage>
    {
        private readonly IEmailSender _emailSender;
        public EmailConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }   

        public async Task Consume(ConsumeContext<EmailMessage> context)
        {
            var message = context.Message;
            var request = new EmailRequest<string>
            {

                To = message.To,
                Subject = message.Subject,
                Body = message.Body
            };

            await _emailSender.SendEmailAsync(request);
        }
    }
}
