using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        #region Fields & Constructors
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IMailHelper _mailHelper;

        public CustomerController(IUserHelper userHelper,
                                  IImageHelper imageHelper,
                                  IConverterHelper converterHelper,
                                  ICustomerRepository customerRepository,
                                  IFlashMessage flashMessage,
                                  IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _customerRepository = customerRepository;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
        }
        #endregion

        #region GET

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public ActionResult Index()
        {
            //var dataContext = _userHelper.Get<Customer>().ToList();

            var dc = _customerRepository.GetAll();

            return View(dc);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = await _userHelper.GetUserByIdAsync(id);

            if (item == null)
                return RedirectToAction("Error404", "Errors");

            return View(item);
        }

        [Authorize(Roles = "Admin,Clerk,Doctor,Customer")]
        public async Task<IActionResult> Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = await _userHelper.GetUserByIdAsync(id);

            if (id == null)
                return RedirectToAction("Error404", "Errors");

            var model = _converterHelper.ToCustomerViewModel(item as Customer);

            ViewBag.IsDisabled = true;

            return View(model);
        }
        #endregion

        #region POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "customer");
                    }

                    try
                    {
                        var customer = _converterHelper.ToCustomer(model, imageId, true);

                        await _userHelper.AddUserAsync(customer, "Cinel123!");
                        await _userHelper.AddUserToRoleAsync(customer, "Customer");

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(customer);

                        string tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {

                            userid = customer.Id,
                            token = myToken

                        }, protocol: HttpContext.Request.Scheme);

                        Response response = _mailHelper.SendEmail(model.Email, "Account confirmation", $"<h2>Welcome to Medpharma!</h2>" +
                            $"To confirm you account, click the link below:</br></br><a href = \"{tokenLink}\"><h3> --> Confirm my account <--</h3> </a></br></br>");

                        if (response.IsSuccess)
                        {
                            _flashMessage.Confirmation("Confirmation email has been sent", "New Client:");
                        }

                        return RedirectToAction("Index");
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
                _flashMessage.Danger("Error! This email is already in use");
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk,Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CustomerViewModel model)
        {
            if (id != model.Id)
                return RedirectToAction("Error404", "Errors");


            if (ModelState.IsValid)
            {
                var userExists = await _userHelper.GetUserByEmailAsync(model.Email);
                if (userExists == null || userExists.Id == model.Id)
                {
                    try
                    {
                        Guid imageId = model.ImageId;

                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                            imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "customer");

                        var customer = _converterHelper.ToCustomer(model, imageId, false);
                        var user = await _userHelper.GetUserByIdAsync(customer.Id) as Customer;

                        user.Document = customer.Document;
                        user.Email = customer.Email;
                        user.FirstName = customer.FirstName;
                        user.LastName = customer.LastName;
                        user.Address = customer.Address;
                        user.PhoneNumber = customer.PhoneNumber;
                        user.Sex = customer.Sex;
                        user.DateOfBirth = customer.DateOfBirth;
                        user.HasInsurance = customer.HasInsurance;
                        user.Weight = customer.Weight;
                        user.Height = customer.Height;
                        user.MedicalInfo = customer.MedicalInfo;
                        user.ImageId = customer.ImageId;

                        await _userHelper.UpdateUserAsync(user);
                    }
                    catch (Exception ex)
                    {
                        
                        if (!await _userHelper.ExistAsync(model.Id))
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        
                        if (ex.InnerException.Message.ToLower().Contains("document"))
                        {
                            ViewBag.IsDisabled = true;
                            _flashMessage.Danger("An account with that Citizen ID already exists.", "Error!");
                            return View(model);
                        }
                        _flashMessage.Danger("An unknown error has occured.", "Error!");
                        return View(model);
                    }
                    _flashMessage.Confirmation("Changes Saved.");

                    return RedirectToAction("Edit", "Customer", new { id = model.Id });
                }
            }
            return View(model);
        }

        #endregion
    }
}
