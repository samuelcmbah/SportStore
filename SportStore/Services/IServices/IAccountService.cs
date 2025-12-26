using Microsoft.AspNetCore.Identity;
using SportStore.Models.ViewModels;

namespace SportStore.Services.IServices
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model, string scheme);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<bool> IsEmailInUseAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task ResendConfirmationLinkAsync(string email, string scheme);

    }

}
