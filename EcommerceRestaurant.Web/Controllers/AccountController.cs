
namespace EcommerceRestaurant.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entities;
    using Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using EcommerceRestaurant.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;

    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly IUserHelper userManager;

        public AccountController(SignInManager<User> signInManager, IUserHelper userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(
                    model.Username,
                    model.Password,
                    model.RemenberMe,
                    false);

                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return this.Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login.");

            return this.View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };

                    var resultRegister = await this.userManager.AddUserAsync(user, model.Password);
                    if (resultRegister != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return this.View(model);
                    }

                    var resultLogin = await this.signInManager.PasswordSignInAsync(
                        model.Username, 
                        model.Password, 
                        true, 
                        false);
                    if (resultLogin.Succeeded)
                    {
                        return this.RedirectToAction("Index", "Home");
                    }

                    this.ModelState.AddModelError(string.Empty, "The user couldn't be login.");
                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The username is already registered.");
            }
            return this.View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser()
        {
            var user = await this.userManager.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var response = await this.userManager.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        this.ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(model);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await this.userManager.ChangePasswordUserAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return this.View(model);
        }

    }
}