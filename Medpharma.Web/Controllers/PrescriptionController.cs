
using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        #region Fields & Constructor
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IOrderRepository _orderRepository;
        private readonly IExamRepository _examRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository,
                                      IAppointmentRepository appointmentRepository,
                                      IMedicineRepository medicineRepository,
                                       IOrderRepository orderRepository,
                                       IFlashMessage flashMessage,
                                      IExamRepository examRepository,
                                      IUserHelper userHelper,
                                      IConverterHelper converterHelper,
                                      IConfiguration configuration)

        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
            _medicineRepository = medicineRepository;
            _orderRepository = orderRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _examRepository = examRepository;
            _converterHelper = converterHelper;
            _configuration = configuration;
        }
        #endregion

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public async Task<IActionResult> TestIndex()
        //{
        //    var datacontext = await _appointmentRepository.GetByIdWithFKs(1);

        //    List<Medicine> apptMedicines = new List<Medicine>();

        //    foreach (var item in datacontext.Prescriptions)
        //    {
        //        var itemToAdd = await _medicineRepository.GetByIdAsync((int)item.MedicineId);
        //        apptMedicines.Add(itemToAdd);
        //    }
        //    return View(apptMedicines);
        //}

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyPrescriptions()
        {
            var customer = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var userApointments = await _appointmentRepository.GetAppointmentsByUserAsync(customer.Id);
            var userRemainingAppointmentMedicine = await _appointmentRepository.GetAppointmentsRemainingByUserAsync(customer.Id);


            var model = new FillPrescriptionViewModel { Appointments = userApointments.Where(i => i.Finished == true && i.Prescriptions.Any(n => n.MedicineId != null)).ToList(), AppointmentsRemaining = userRemainingAppointmentMedicine };

            return View(model);

        }

        [Authorize(Roles = "Doctor")]
        public IActionResult CreateMedicinePrescriptionAsync(int id)
        {
            var prescriptions = _prescriptionRepository.GetByAppointment(id);

            //int[] prescriptionsUsed = new int[] { };
            List<int> prescriptionsUsed = new List<int>();

            foreach (Prescription p in prescriptions)
            {
                if (p.MedicineId != null)
                {
                    prescriptionsUsed.Add((int)p.MedicineId);
                }
            }

            ViewData["Medicines"] = _medicineRepository.GetAll().Where(t => !prescriptionsUsed.Contains(t.Id)).OrderBy(t => t.Name);

            PrescriptionViewModel model = new PrescriptionViewModel();

            model.AppointmentId = id;

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> FillPrescription(FillPrescriptionViewModel model)
        {
            var appointment = await _appointmentRepository.GetByIdWithFKs(model.AppointmentId);

            foreach (var prescription in appointment.Prescriptions)
            {
                await _orderRepository.AddPrescriptionToOrderAsync(prescription.Medicine, prescription.Quantity, appointment.Customer);
            }

            //appointment.PrescriptionsFilled = 1;

            await _appointmentRepository.UpdateAsync(appointment);

            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateMedicinePrescription(PrescriptionViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                //Prescription
                Random random = new Random();
                var prescriptionNumber = random.Next(1111111, 9999999);

                if (model.MedicineId != null)
                {
                    var prescription = new Prescription()
                    {
                        MedicineId = model.MedicineId,
                        AppointmentId = id,
                        Number = prescriptionNumber,
                        Observations = model.Observations,
                        ExpirationDate = DateTime.Now.Date.AddMonths(6),
                        Quantity = model.Quantity,
                    };

                    await _prescriptionRepository.CreateAsync(prescription);
                }

                return RedirectToAction("Edit", "Appointment", new { id = model.AppointmentId });
            }
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult CreateExamPrescription(int id)
        {
            ViewData["Exams"] = _examRepository.GetAll().OrderBy(e => e.Name);

            PrescriptionViewModel model = new PrescriptionViewModel();

            model.AppointmentId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateExamPrescription(PrescriptionViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                //Prescription
                Random random = new Random();
                var prescriptionNumber = random.Next(1111111, 9999999);

                if (model.ExamId != null)
                {
                    var prescription = new Prescription()
                    {
                        ExamId = model.ExamId,
                        AppointmentId = id,
                        Number = prescriptionNumber,
                        Observations = model.Observations,
                        ExpirationDate = DateTime.Now.Date.AddMonths(6),
                        Quantity = model.Quantity,
                    };

                    await _prescriptionRepository.CreateAsync(prescription);
                }

                return RedirectToAction("Edit", "Appointment", new { id = model.AppointmentId });
            }
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Exams"] = _examRepository.GetAll().OrderBy(e => e.Name);

            ViewData["Medicines"] = _medicineRepository.GetAll().OrderBy(e => e.Name);

            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var item = await _prescriptionRepository.GetByIdAsync(id.Value);

            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var model = _converterHelper.ToPrescriptionViewModel(item);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int id, PrescriptionViewModel model)
        {
            if (id != model.Id)
            {
                return RedirectToAction("Error404", "Errors");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var prescription = _converterHelper.ToPrescription(model, false);

                    await _prescriptionRepository.UpdateAsync(prescription);

                    _flashMessage.Info("Changes Saved");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _prescriptionRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Appointment", new { id = model.AppointmentId });
            }

            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var prescription = await _prescriptionRepository.GetByIdAsync(id.Value);

            if (prescription == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            if (prescription.ExamId == null)
            {
                prescription.Medicine = await _medicineRepository.GetByIdAsync(prescription.MedicineId.Value);
            }
            else
            {
                prescription.Exam = await _examRepository.GetByIdAsync(prescription.ExamId.Value);
            }

            return View(prescription);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(int precriptionId)
        {
            var item = await _prescriptionRepository.GetByIdAsync(precriptionId);

            await _prescriptionRepository.DeleteAsync(item);

            _flashMessage.Info("Item deleted");

            return RedirectToAction("Edit", "Appointment", new { id = item.AppointmentId });

        }


    }
}
