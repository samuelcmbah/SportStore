using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models.ViewModels;
using SportStore.Services;
using SportStore.Services.IServices;
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
            return Json(!await accountService.IsEmailInUseAsync(email));

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

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            if (!ModelState.IsValid) return View(model);

            var result = await accountService.LoginAsync(model);

            if (result.Succeeded)
            {
                // If returnUrl is valid and local, redirect there. Otherwise, redirect to home.
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            // Handle specific login failures
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your email has not been confirmed. Please check your inbox.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
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
    }
}
