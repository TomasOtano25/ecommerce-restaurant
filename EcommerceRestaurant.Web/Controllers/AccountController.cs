
namespace EcommerceRestaurant.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entities;
    using Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using EcommerceRestaurant.Web.Helpers;

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


    }
}