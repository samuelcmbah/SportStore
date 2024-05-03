using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SportStore.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly EmailService emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                    EmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            return Json($"Email '{email}' is already in use");
        }

        [HttpPost]
        public async Task<IActionResult> Logout(RegisterViewModel model)
        {
             await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return RedirectToAction("index", "home");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            ViewData["Title"] = "Email not confirmed";

            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //create the user using the provided email and password//and then signs the user in
                var user = new IdentityUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await userManager.CreateAsync(user, model.Password);
                //// Schedule a cleanup task for unconfirmed users (optional)
                //var confirmationExpiration = TimeSpan.FromSeconds(30); // Set your desired timeframe
                //await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(confirmationExpiration));

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token, Request.Scheme });

                    await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink, Request.Scheme);
                    return View("RegistrationSuccessful");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]

        public IActionResult Login(string  returnUrl = "/" )
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model )
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                
                if (user != null )
                {
                    var verifiedCredentials = await userManager.CheckPasswordAsync(user, model.Password);

                    if (!user.EmailConfirmed && verifiedCredentials)
                    {
                        ModelState.AddModelError("", "Email not confirmed");
                        return View(model);
                    }
                    
                }
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if(!model.ReturnUrl.IsNullOrEmpty())
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }
                        
                    }
                }
                //sth must have failed
                ModelState.AddModelError("", "invalid login attempt");
            }
            return View();
        }
    }
}
