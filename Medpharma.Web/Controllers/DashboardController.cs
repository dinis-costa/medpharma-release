using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IClerkRepository _clerkRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAdmissionRepository _admissionRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;


        public DashboardController(UserManager<User> userManager, IUserHelper userHelper, ICustomerRepository customerRepository, IClerkRepository clerkRepository,
            IDoctorRepository doctorRepository, IOrderRepository orderRepository, IMedicineRepository medicineRepository, IAppointmentRepository appointmentRepository,
            IAdmissionRepository admissionRepository, IPrescriptionRepository prescriptionRepository)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _customerRepository = customerRepository;
            _clerkRepository = clerkRepository;
            _doctorRepository = doctorRepository;
            _orderRepository = orderRepository;
            _medicineRepository = medicineRepository;
            _appointmentRepository = appointmentRepository;
            _admissionRepository = admissionRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel dashboardViewModel = new()
            {
                TotalUsers = _userManager.Users.Count(),

                TotalClerks = _clerkRepository.GetAll().Count(),

                TotalClients = _customerRepository.GetAll().Count(),

                TotalDoctors = _doctorRepository.GetAll().Count(),

                TotalOrders = _orderRepository.GetAll().Count(),

                TotalProducts = _medicineRepository.GetAll().Count(),

                Admissions = _admissionRepository.GetAll().Include("Customer").Where(a => a.AdmissionTime.Date == DateTime.Now.Date).ToList(),

                Orders = _orderRepository.GetAll().Include("Customer").OrderBy(a => a.OrderDate).ToList(),

                TotalOrdersRevenue = (double)_orderRepository.TotalOrderRevenue(),

                TotalAppointmentsRevenue = _appointmentRepository.GetAll().Where(a => a.IsPaid).Select(a => a.Price).Sum(),
            };

            if (User.IsInRole("Doctor"))
            {
                dashboardViewModel.Appointments = _appointmentRepository.GetByDateFromDoctor(DateTime.Now.Date, User.Identity.Name).ToList();
            }
            else if (User.IsInRole("Clerk"))
            {
                dashboardViewModel.Appointments = _appointmentRepository.GetByDate(DateTime.Now.Date).ToList();
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (User.IsInRole("Clerk"))
            {
                dashboardViewModel.WareHouse = _clerkRepository.GetClerkType(user);
            }

            if (User.IsInRole("Customer"))
            {
                dashboardViewModel.OrdersByCustomer = _orderRepository.GetAll().Include("Items").Where(t => t.Customer == user).ToList();
            }

            await CountMedicinesWithPrescriptionSold(dashboardViewModel);

            CountProductsWithPrescriptionRequired(dashboardViewModel);

            return View(dashboardViewModel);
        }

        private async Task CountMedicinesWithPrescriptionSold(DashboardViewModel model)
        {
            var counter = 0;
            var total = 0;
            var orders = await _orderRepository.GetAll().Include(o => o.Items).ThenInclude(p => p.Product).ToListAsync();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    if (item.Product.NeedsPrescription == true)
                    {
                        counter++;
                    }

                    total += 1;
                }
            }

            model.TotalMedicinesSoldWithoutPrescription = total - counter;

            model.TotalMedicinesSoldWithPrescription = counter;
        }

        private void CountProductsWithPrescriptionRequired(DashboardViewModel model)
        {
            var counterNeedsPrescription = 0;
            var counterDontNeedsPrescription = 0;
            var medicines = _medicineRepository.GetAll().ToList();

            foreach (var medicine in medicines)
            {
                if (medicine.NeedsPrescription == true)
                {
                    counterNeedsPrescription++;
                }
                else
                {
                    counterDontNeedsPrescription++;
                }
            }

            model.TotalMedicinesWithPrescription = counterNeedsPrescription;

            model.TotalMedicinesWithoutPrescription = counterDontNeedsPrescription;
        }
    }
}
