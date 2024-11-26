using System.Net;
using System.Net.Mail;
using OnlineLibrary.Core.Domain.Settings;

namespace OnlineLibrary.Presentation.Web.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(EmailContent emailContent)
    {
        SmtpClient smtpClient = new SmtpClient(_config["EmailSettings:SmtpHost"])
        {
            Port = int.Parse(_config["EmailSettings:SmtpPort"]!),
            EnableSsl = true,
            Credentials = new NetworkCredential(_config["EmailSettings:EmailFrom"], _config["EmailSettings:SmtpPass"])
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:EmailFrom"]!),
            Subject = emailContent.Subject,
            Body = emailContent.Message,
            IsBodyHtml = true,
            To = { emailContent.To }
        };

        smtpClient.Send(mailMessage);
    }
}