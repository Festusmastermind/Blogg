using Blogg.Models;
using Blogg.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //injecting the usermanager and signInManger in the contructor
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<RegisterViewModel> _logger;

        [TempData]
        public string StatusMessage { get; set; }
        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, ILogger<RegisterViewModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //creating an instance of the Identityuser class where the table data is 
                var user = new ApplicationUser { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    FirstName = model.FirstName, 
                    LastName = model.LastName
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("dashboard", "post");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            
            //if we get this far, something is wrong with the form, thus redisplay the form..with the data.
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    if (!string.IsNullOrEmpty(returnurl)){
                        //  return Redirect(returnurl); This encourages an open redirect attack manual url redirection 
                        return LocalRedirect(returnurl);
                    }
                    else
                    {
                        StatusMessage = "Welcome" + " " + model.Email;
                        TempData["message"] = StatusMessage;
                        return RedirectToAction("dashboard", "post");
                    }
                    //return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            //if it gets to the line then something is wrong with the form then redisplay form
            return View(model);
        }
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            _logger.LogInformation("User Logged Out.");
            if(returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                StatusMessage = "You have logged out succesfully";
                TempData["message"] = StatusMessage;
                return RedirectToAction("index", "home");
            }
            
        }
        //public async Task<IActionResult> Logout()
        //{
        //    await signInManager.SignOutAsync();
        //    return RedirectToAction("home", "index");
        //}

    }
}
