using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class ClerkController : Controller
    {
        #region Fields & Constructors
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IFlashMessage _flashMessage;

        public ClerkController(IUserHelper userHelper,
                                  IImageHelper imageHelper,
                                  IConverterHelper converterHelper,
                                   IOrderRepository orderRepository,
                                   IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _orderRepository = orderRepository;
            _flashMessage = flashMessage;
        }
        #endregion

        #region GET

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var dataContext = _userHelper.Get<Clerk>().ToList();
            return View(dataContext);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = await _userHelper.GetUserByIdAsync(id);

            if (item == null)
                return RedirectToAction("Error404", "Errors");

            return View(item);
        }

        [Authorize(Roles = "Admin, Clerk")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = await _userHelper.GetUserByIdAsync(id);

            if (id == null)
                return RedirectToAction("Error404", "Errors");

            var model = _converterHelper.ToClerkViewModel(item as Clerk);

            ViewBag.IsDisabled = true;

            return View(model);
        }
        #endregion

        #region POST

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClerkViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "clerk");
                    }

                    var customer = _converterHelper.ToClerk(model, imageId, true);
                    customer.EmailConfirmed = true;

                    await _userHelper.AddUserAsync(customer, "Cinel123!");
                    await _userHelper.AddUserToRoleAsync(customer, "Clerk");

                    return RedirectToAction("Index");
                }
            }
            //_flashMessage.Danger("Error! There's already a user with that email!");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> Edit(string id, ClerkViewModel model)
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
                            imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "clerk");

                        var customer = _converterHelper.ToClerk(model, imageId, false);

                        var user = await _userHelper.GetUserByIdAsync(customer.Id) as Clerk;

                        user.Document = customer.Document;
                        user.Email = customer.Email;
                        user.FirstName = customer.FirstName;
                        user.LastName = customer.LastName;
                        user.ImageId = customer.ImageId;

                        await _userHelper.UpdateUserAsync(user);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await _userHelper.ExistAsync(model.Id))
                        {
                            //_flashMessage.Danger("Error! There's was an error processing your request.");
                            return RedirectToAction(nameof(Index));
                        }
                        else
                            throw;
                    }
                    _flashMessage.Confirmation("Changes Saved");

                    return RedirectToAction("Edit", "Clerk", new { id = model.Id });
                }
            }
            //_flashMessage.Danger("Error! There's was an error processing your request.");
            return View(model);
        }

        #endregion

        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> Orders()
        {
            var myOrders = await _orderRepository.GetAll().Include(o => o.Customer).ToListAsync();

            return View(myOrders);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> OrderDetails(int? orderId)
        {
            if (orderId == null)
                return NotFound();

            var order = await _orderRepository.GetOrderById(orderId.Value);

            var model = new OrderDetailsViewModel { OrderId = order.Id, OrderSent = order.OrderSent, Customer = order.Customer, OrderDetailList = order.Items.ToList() };

            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> ConfirmOrder(int? orderId)
        {
            if (orderId == null)
                return NotFound();

            await _orderRepository.DeliveryOrder(orderId.Value);

            return RedirectToAction("Orders");
        }
    }
}
