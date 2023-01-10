using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUserHelper userHelper, ICustomerRepository customerRepository, IConfiguration configuration,
            IMailHelper mailHelper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _customerRepository = customerRepository;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _userManager = userManager;
            _roleManager = roleManager;
            _flashMessage = flashMessage;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    user = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        Document = model.Document,
                        PhoneNumber = model.PhoneNumber
                    };

                    try
                    {
                        var result = await _userHelper.AddUserAsync(user, model.Password);

                        await _userHelper.AddUserToRoleAsync(user, "Customer");

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "Unable to create new user");
                            return View(model);
                        }

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                        string tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {

                            userid = user.Id,
                            token = myToken

                        }, protocol: HttpContext.Request.Scheme);

                        Response response = _mailHelper.SendEmail(model.Username, "Account confirmation", $"<h2>Welcome to Medpharma!</h2>" +
                            $"To confirm you account, click the link below:</br></br><a href = \"{tokenLink}\"><h3> --> Confirm my account <--</h3> </a></br></br>");

                        if (response.IsSuccess)
                        {
                            TempData["success"] = $"Confirmation email has been sent";
                        }
                        _flashMessage.Confirmation("Check your email inbox to finish account creation.", "Success!");
                        return RedirectToAction("Login");
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.ToLower().Contains("document"))
                        {
                            _flashMessage.Danger("An account with that Citizen ID already exists.", "Error!");
                            return View(model);
                        }
                        _flashMessage.Danger("An unknown error has occured.", "Error!");
                        return View(model);
                    }

                    
                }
                _flashMessage.Danger("An account with that email already exists.", "Error!");
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                _flashMessage.Danger("A unknown error has occured. Try again later, if the issue persists contact our support line.", "Error!");
            }

            _flashMessage.Confirmation("Email confirmed. You can now login.", "Success!");
            return View("Login");

        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);


            if (User.IsInRole("Customer"))
            {
                return RedirectToAction("Edit", "Customer", new { id = user.Id });
            }

            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("Edit", "Doctor", new { id = user.Id });
            }

            if (User.IsInRole("Clerk"))
            {
                return RedirectToAction("Edit", "Clerk", new { id = user.Id });
            }

            if (User.IsInRole("Admin"))
            {
                var admin = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                var model = new EditProfileViewModel();

                if (user != null)
                {
                    model.FirstName = admin.FirstName;
                    model.LastName = admin.LastName;
                    //model.Document = admin.Document;
                    //model.Address = customer.Address;
                }

                return View(model);
            }

            return NotFound();

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (customer != null)
                {
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;

                    var response = await _userHelper.UpdateUserAsync(customer);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "Changes saved!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (customer != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(customer, model.OldPassword, model.NewPassword).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        _flashMessage.Confirmation("Changes saved.");
                        return this.RedirectToAction("EditProfile");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);

                    }
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Sorry, but this email was not found...");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "Medpharma - Password Reset", $"<h1>Password Reset</h1>" +
                $"Almost done! Just click the link below to reset the password:</br></br>" +
                $"<a href = \"{link}\"> -->Reset Password <-- </a>");

                if (response.IsSuccess)
                {
                    this.ViewBag.Message = "Instructions to reset password have been sent to your email";
                }

                return this.View();

            }

            return this.View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Your password has been reset.";
                    return View();
                }

                this.ViewBag.Message = "Error while resetting the password. Please try again later.";
                return View(model);
            }

            this.ViewBag.Message = "User not found.";

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.OrderBy(e => e.FirstName);

            return View(users);
        }

    }
}
