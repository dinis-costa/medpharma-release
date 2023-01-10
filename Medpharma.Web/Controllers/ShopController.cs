using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ShopController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserHelper _userHelper;
        private readonly ICheckoutHelper _checkoutHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;

        public ShopController(
            IMedicineRepository medicineRepository,
            IOrderRepository orderRepository,
            IUserHelper userHelper,
            ICheckoutHelper checkoutHelper,
            IAppointmentRepository appointmentRepository,
            IConverterHelper converterHelper,
            IConfiguration configuration
            )
        {
            _medicineRepository = medicineRepository;
            _orderRepository = orderRepository;
            _userHelper = userHelper;
            _checkoutHelper = checkoutHelper;
            _appointmentRepository = appointmentRepository;
            _converterHelper = converterHelper;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var products = _medicineRepository.GetAll().Where(m => m.NeedsPrescription == false).OrderBy(p => p.Name);

            return View(products);
        }

        public async Task<IActionResult> CartIndex()
        {
            var orderDetailsTem = await _orderRepository.GetMedicinesCartByUserAsync(User.Identity.Name);
            return View(orderDetailsTem);
        }

        public async Task<IActionResult> AddMedicine(int? id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id.Value);

            var customer = await _userHelper.GetUserByEmailAsync(User.Identity.Name) as Customer;
            await _orderRepository.AddPrescriptionToOrderAsync(medicine, 1, customer);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> NumberCartItems()
        {
            var userName = User.Identity.Name;
            return Json(await _orderRepository.CountCartItems(userName));
        }

        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);

            return RedirectToAction("CartIndex");
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);

            return RedirectToAction("CartIndex");
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.DeleteDetailTempAsync(id.Value);

            return RedirectToAction("CartIndex");
        }

        public async Task<IActionResult> CheckOutInfo()
        {
            //var user = await _userHelper.GetUserByIdAsync(_configuration["User:Id"]) as Customer;
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name) as Customer;

            if (user == null)
                return NotFound();

            var orderDetailsTem = await _orderRepository.GetMedicinesCartByUserAsync(User.Identity.Name);

            var model = new CheckoutViewModel { Customer = user, OrderDetailTempList = orderDetailsTem };

            return View(model);
        }

        public async Task<IActionResult> CheckOutPrescriptionInfo(int? appointmentId, string? prescriptionType)
        {
            // METER ORIGEM DO REQ PARA SABER SE È REMAINING OU NÂO, METER PARAM NO BUTTON
            if (appointmentId == null || prescriptionType == null)
                return NotFound();

            switch (prescriptionType.ToLower().Trim())
            {
                case "standard":
                    var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId.Value);

                    var model = await _checkoutHelper.GetPrescriptionRemainingAsync(appointment);

                    return View(model);

                case "remaining":
                    var appointmentRem = await _appointmentRepository.GetAppointmentRemainingByIdAsync(appointmentId.Value);

                    var appointmentModel = _converterHelper.ToAppointmentModel(appointmentRem);

                    var remainingModel = await _checkoutHelper.GetPrescriptionRemainingAsync(appointmentModel);

                    ViewBag.IsRemaining = true;

                    return View(remainingModel);
                default:
                    return BadRequest();
            }
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name) as Customer;
            //var user = await _userHelper.GetUserByIdAsync(_configuration["User:Id"]) as Customer;

            if (user == null)
                return NotFound();

            var myOrders = await _orderRepository.GetOrderAsync(user.Id);

            return View(myOrders.ToList());
        }

        public async Task<IActionResult> MyOrderDetails(int? orderId)
        {
            if (orderId == null)
                return NotFound();

            var order = await _orderRepository.GetOrderById(orderId.Value);

            var model = new OrderDetailsViewModel { OrderId = order.Id, DeliveryDate = order.DeliveryDate == null ? null : (DateTime)order.DeliveryDate, OrderSent = order.OrderSent, Customer = order.Customer, OrderDetailList = order.Items.ToList() };

            return View(model);
        }

    }
}
