using Microsoft.AspNetCore.Identity;
using SportStore.Models.ViewModels;
using SportStore.Services.IServices;
using System.Net;

namespace SportStore.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await userManager.FindByEmailAsync(email) != null;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string scheme)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            var confirmationLink =  await GenerateConfirmationLinkAsync(user, scheme);

            await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            // Alternatively, we could using a library like hangfire
            // Enqueue the job instead of calling the service directly
            //_backgroundJobClient.Enqueue<IEmailService>(
            //    emailService => emailService.SendConfirmationEmailAsync(user.Email, confirmationLink)
            //);

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return SignInResult.Failed;
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                // This result can be checked in the controller to show a specific message
                return SignInResult.NotAllowed;
            }

            
            var result = await signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: false);

            return result;
        }


        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task ResendConfirmationLinkAsync(string email, string scheme)
        {
            var user = await userManager.FindByEmailAsync(email);

            if(user != null && !user.EmailConfirmed)
            {
                var confirmationLink =   await GenerateConfirmationLinkAsync(user, scheme);
                await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
            }
        }

        private async Task<string> GenerateConfirmationLinkAsync(IdentityUser user,  string scheme)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var request = httpContextAccessor.HttpContext!.Request;
            var encodedToken = WebUtility.UrlEncode(token);
            return $"{scheme}://{request.Host}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";
        }
    }

}
