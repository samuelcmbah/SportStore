using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.ViewModels.Auth;
using System.ComponentModel.DataAnnotations;

namespace SportStore.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            if (!await accountService.IsEmailInUseAsync(email))
            {
                return Json(true);
            }
            else
            {
                return Json($"Email '{email}' is already in use");
            }
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await accountService.RegisterAsync(model, Request.Scheme);
            if (result.Succeeded)
            {
                return View("ConfirmationEmailSent");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

       
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string?  returnUrl = null )
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;

            if (!ModelState.IsValid) 
                return View(model);

            var loginResult = await accountService.LoginAsync(model);

            if (loginResult.SignInResult.Succeeded)
            {
                // If returnUrl is valid and local, redirect there. Otherwise, redirect to home.
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                if (loginResult.IsAdmin)
                {
                    return RedirectToAction("Index", "Products", new {area = "Admin"});
                }

                return RedirectToAction("Index", "Home");
            }

            if (loginResult.SignInResult.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your email has not been confirmed. Please check your inbox");
                // Pass a signal to the view to show the resend link
                ViewData["ShowResendLink"] = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            

            

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await accountService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail"); // Show a "Thank you for confirming" page
            }

            ViewBag.ErrorTitle = "Email Confirmation Failed";
            ViewBag.ErrorMessage = "The confirmation link is invalid or has expired.";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendConfirmationLink()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationLink(ResendConfirmationLinkViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await accountService.ResendConfirmationLinkAsync(model.Email, Request.Scheme);

            
            return RedirectToAction("ConfirmationEmailSent");
        }
    }
}
