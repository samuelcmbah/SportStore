using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SportStore.Services;

public class EmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string email, string? confirmationLink, string scheme)
    {
        string domain = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? "localhost:7000" : "SportStore.com";
        var absoluteConfimationLink = $"{scheme}://{domain}{confirmationLink}";
        _logger.LogWarning(absoluteConfimationLink);

        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("SportStore", _emailSettings.SmtpUsername));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Email Confirmation";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>Please confirm your email by clicking the link below:</p><p><a href='{absoluteConfimationLink}'>Confirm Email</a></p>" +
            $"<p>safely ignore if you did not sign up for a SportStore account</p>" +
            $"<p>Best,</p>" +
            $"<p>SportStore</>"
        };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true);
                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                await client.SendAsync(message);
                _logger.LogInformation("Email sent successfully.");
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while sending email: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
