namespace SportStore.Services.IServices
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string to, string confirmationLink);
    }
}
