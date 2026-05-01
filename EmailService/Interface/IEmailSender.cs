using EmailService.DTO;

namespace EmailService.Interface
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailRequest<string> request);
        Task SendEmailWithAttachmentAsync(EmailRequest<string> request, Stream fileStream, string fileName);
    }
}
