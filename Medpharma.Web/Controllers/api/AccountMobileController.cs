using Medpharma.Mobile.Models;
using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medpharma.Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMobileController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AccountMobileController(ICustomerRepository customerRepository,
                                 IUserHelper userHelper,
                                 UserManager<User> userManager,
                                 IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _userHelper = userHelper;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            return Json(_customerRepository.GetAll());
        }

        //[HttpPost]
        //[Route("Register")]
        //public async Task<IActionResult> Register(RegisterBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = new Customer()
        //    {
        //        UserName = model.Email,
        //        Email = model.Email,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Document = model.Document,
        //        EmailConfirmed = true, //TODO: Confirmar por email.
        //    };



        //    var result = await _userHelper.AddUserAsync(user, model.Password);


        //    if (!result.Succeeded)
        //    {
        //        //return GetErrorResult(result);
        //        return BadRequest(new ProblemDetails
        //        {
        //            Title = "Something went wrong...",
        //        });
        //    }

        //    await _userHelper.AddUserToRoleAsync(user, "Customer");

        //    return Ok();
        //}

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(BindingLogin model)
        {
            if (ModelState.IsValid)
            {
                var loginModel = new Medpharma.Web.Models.LoginViewModel();
                loginModel.Username = model.Email.ToString();
                loginModel.Password = model.Password.ToString();
                loginModel.RememberMe = true;

                var result = await _userHelper.LoginAsync(loginModel);

                var user = await _userHelper.GetUserByEmailAsync(loginModel.Username);

                if (!result.Succeeded)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Failed to login...",
                    });
                }
                return Ok(user);
            }
            return BadRequest();
        }

    }
}
