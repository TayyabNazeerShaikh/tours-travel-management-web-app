using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace ToursAndTravelsManagement.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailSettings.SMTPServer, _emailSettings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailSettings.SMTPUsername, _emailSettings.SMTPPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
} 