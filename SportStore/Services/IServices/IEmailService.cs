namespace SportStore.Services.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string html);
        Task SendConfirmationEmailAsync(string to, string confirmationLink);
    }
}
