namespace EcommerceRestaurant.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entities;
    using Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Data.Repositories;

    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly IUserHelper userManager;
        private readonly ICountryRepository countryRepository;
        private readonly IConfiguration configuration;

        public AccountController(SignInManager<User> signInManager, 
            IUserHelper userManager,
            ICountryRepository countryRepository,
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.countryRepository = countryRepository;
            this.configuration = configuration;
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
            var model = new RegisterViewModel
            {
                Countries = this.countryRepository.GetComboCountries(),
                Cities = this.countryRepository.GetComboCities(0)
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    var city = await this.countryRepository.GetCityAsync(model.CityId);

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        CityId = model.CityId,
                        City = city
                    };

                    var resultRegister = await this.userManager.AddUserAsync(user, model.Password);
                    if (resultRegister != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return this.View(model);
                    }

                    var myToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = this.Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    var mailSender = new MailHelper(configuration);

                    mailSender.SendEmail(model.Username, "Email Confirmation",
                        $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>"
                    );

                    this.ViewBag.Message = "The instructions to allow your user has been sent to email.";

                    return this.View(model);
                

                    /*var resultLogin = await this.signInManager.PasswordSignInAsync(
                        model.Username, 
                        model.Password, 
                        true, 
                        false);
                    if (resultLogin.Succeeded)
                    {
                        await this.userManager.AddToRoleUserAsync(user, "Customer");
                        return this.RedirectToAction("Index", "Home");
                    }

                    this.ModelState.AddModelError(string.Empty, "The user couldn't be login.");
                    return this.View(model);*/
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
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;

                var city = await this.countryRepository.GetCityAsync(user.CityId);
                if (city != null)
                {
                    var country = await this.countryRepository.GetAsync(city);
                    if (country != null)
                    {
                        model.CountryId = country.Id;
                        model.Cities = this.countryRepository.GetComboCities(country.Id);
                        model.Countries = this.countryRepository.GetComboCountries();
                        model.CityId = user.CityId;
                    }
                }
            }

            model.Cities = this.countryRepository.GetComboCities(model.CountryId);
            model.Countries = this.countryRepository.GetComboCountries();

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
                    var city = await this.countryRepository.GetCityAsync(model.CityId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.CityId = model.CityId;
                    user.City = city;

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

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        var results = this.userManager.GenerateToken(user);

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return this.BadRequest();
        }

        public async Task<JsonResult> GetCities(int countryId)
        {
            var country = await this.countryRepository.GetAsync(countryId);
            return this.Json(country.Cities.OrderBy(c => c.Name));
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await this.userManager.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await this.userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return this.NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return this.View(model);
                }

                var myToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var link = this.Url.Action("ResetPassword", "Account", new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                var mailSender = new MailHelper(configuration);
                mailSender.SendEmail(model.Email, "Password Reset", 
                    $"<h1>Recover Password</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>"
                );

                this.ViewBag.Message = "The instructions to recover your password has been sent to email.";

                return this.View();
            }

            return View(model);
        }


        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        //TODO: Colocar el ModelState
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await this.userManager.GetUserByNameAsync(model.Username);
            if (user != null)
            {
                var result = await this.userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return this.View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found";
            return View(model);
        }

    }
}