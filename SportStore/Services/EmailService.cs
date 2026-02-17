using Microsoft.Extensions.Options;
using Resend; // Main Resend namespace
using SportStore.Configurations;
using SportStore.Services.IServices;

// Make sure you have the Resend SDK installed: dotnet add package Resend
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly ResendEmailSettings _emailSettings;
    private readonly IResend _resend;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<ResendEmailSettings> options,
        IResend resend)
    {
        _logger = logger;
        _emailSettings = options.Value;
        _resend = resend;
    }

    public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
    {
        var html = $@"
            <p>Hello there, thank you for signing up on SportStore.</p>
            <p>Please confirm your email by clicking the link below:</p>
            <div style='margin-top:20px;'>
                <a href='{confirmationLink}'
                   style='display:inline-block;padding:10px 20px;font-size:16px;
                   color:#fff;background-color:#007bff;text-decoration:none;
                   border-radius:5px;'>Confirm Email</a>
            </div>
            <p>If you did not sign up for a SportStore account, you can safely ignore this email.</p>
            <p>Best,</p>
            <p>The SportStore Team</p>";

        try
        {
            _logger.LogInformation($"Sending confirmation email to {to}");

            var message = new EmailMessage
            {
                From = $"SportStore <{_emailSettings.FromEmail}>",
                To = to,
                Subject = "Confirm your SportStore email",
                HtmlBody = html
            };

            await _resend.EmailSendAsync(message);

            _logger.LogInformation($"Confirmation email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending confirmation email to {Email}", to);
            // Depending on your strategy, you might want to re-throw or handle this
            return;
        }
    }

    public async Task SendEmailAsync(string to, string subject, string html)
    {
        try
        {
            var message = new EmailMessage
            {
                From = $"SportStore <{_emailSettings.FromEmail}>",
                To = to,
                Subject = subject,
                HtmlBody = html
            };

            await _resend.EmailSendAsync(message);
            _logger.LogInformation($"Order Confirmation email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending order confirmation email to {Email}", to);
            // Depending on your strategy, you might want to re-throw or handle this
            return;
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        var html = $@"
        <p>Hi there,</p>
        <p>We received a request to reset the password for your SportStore account.</p>
        <p>Click the button below to choose a new password. This link is valid for <strong>1 hour</strong>.</p>
        <div style='margin-top:20px;margin-bottom:20px;'>
            <a href='{resetLink}'
               style='display:inline-block;padding:12px 24px;font-size:16px;
               color:#fff;background-color:#0056b3;text-decoration:none;
               border-radius:6px;font-weight:600;'>Reset My Password</a>
        </div>
        <p>If you didn't request a password reset, you can safely ignore this email.
           Your password will not change.</p>
        <p>Best,<br/>The SportStore Team</p>";

        try
        {
            _logger.LogInformation("Sending password reset email to {Email}", to);

            var message = new EmailMessage
            {
                From = $"SportStore <{_emailSettings.FromEmail}>",
                To = to,
                Subject = "Reset your SportStore password",
                HtmlBody = html
            };

            await _resend.EmailSendAsync(message);

            _logger.LogInformation("Password reset email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", to);
        }
    }
}

    