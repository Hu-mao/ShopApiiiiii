using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shop.Application.Interfaces.Services;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Shop.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendEmailAsync(
        string email,
        string subject,
        string body)
    {
        var settings = configuration
            .GetSection("EmailSettings");

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                settings["From"],
                settings["Email"]));

        message.To.Add(
            MailboxAddress.Parse(email));

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            settings["Host"],
            int.Parse(settings["Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            settings["Email"],
            settings["Password"]);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }

}