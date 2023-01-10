using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        #region Fields & Constructor
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITimeslotRepository _timeslotRepository;
        private readonly IMedicalScreeningRepository _medicalScreeningRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public AppointmentController(ISpecialityRepository specialityRepository,
                                     IDoctorRepository doctorRepository,
                                     ITimeslotRepository timeslotRepository,
                                     IMedicalScreeningRepository medicalScreeningRepository,
                                     IAppointmentRepository appointmentRepository,
                                     ICustomerRepository customerRepository,
                                     IConverterHelper converterHelper,
                                     IMedicineRepository medicineRepository,
                                     IPrescriptionRepository prescriptionRepository,
                                     IUserHelper userHelper,
                                     IMailHelper mailHelper,
                                     IFlashMessage flashMessage)
        {
            _specialityRepository = specialityRepository;
            _doctorRepository = doctorRepository;
            _timeslotRepository = timeslotRepository;
            _medicalScreeningRepository = medicalScreeningRepository;
            _appointmentRepository = appointmentRepository;
            _customerRepository = customerRepository;
            _converterHelper = converterHelper;
            _medicineRepository = medicineRepository;
            _prescriptionRepository = prescriptionRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }
        #endregion

        #region GET

        public async Task<IActionResult> Index()
        {
            List<Appointment> dc = new();

            if (User.IsInRole("Customer"))
            {
                dc = await _appointmentRepository.GetAppointmentsByCustomer(User.Identity.Name);
            }
            else if (User.IsInRole("Clerk"))
            {
                dc = _appointmentRepository.GetAllWithFKs().ToList();
            }
            else if (User.IsInRole("Doctor"))
            {
                dc = await _appointmentRepository.GetAppointmentsByDoctor(User.Identity.Name);
            }
            else if (User.IsInRole("Admin"))
            {
                dc = await _appointmentRepository.GetAllWithFKs().ToListAsync();
            }

            return View(dc.OrderBy(a => a.Date.Date).ThenBy(a => a.TimeslotId));
        }

        public async Task<IActionResult> Create(string? customerString, int? medicalScreening)
        {
            ViewBag.data = _medicineRepository.GetAll().ToList();

            if (!User.IsInRole("Customer"))
            {
                if (medicalScreening != null)
                {
                    var appt = new Appointment();
                    var ms = await _medicalScreeningRepository.GetByIdWithInfo(medicalScreening.Value);
                    var doctor = await _doctorRepository.GetUserByEmailAsync(User.Identity.Name);

                    if (ms != null)
                    {
                        appt.MedicalScreening = ms;
                        appt.MedicalScreeningId = ms.Id;
                        appt.Customer = ms.Admission.Customer;
                        appt.CustomerId = ms.Admission.CustomerId;
                        appt.Date = ms.Admission.AdmissionTime.Date;
                        appt.DoctorId = doctor.Id;
                        appt.SpecialityId = ms.SpecialityId;
                        appt.Notes = ms.Admission.Notes;
                    }
                    await _appointmentRepository.CreateAsync(appt);

                    ms.IsAccepted = true;
                    await _medicalScreeningRepository.UpdateAsync(ms);

                    var apptId = await _appointmentRepository.GetByMedicalScreeningId(ms.Id);

                    return RedirectToAction("Edit", "Appointment", new { id = apptId.Id });
                }
            }

            // Scheduled Appointment Creation
            if (!string.IsNullOrEmpty(customerString) || User.IsInRole("Customer")) //AR
            {
                Medpharma.Web.Data.Entities.Customer customer = null;

                if (!string.IsNullOrEmpty(customerString))
                    customer = await _customerRepository.GetByIdAsync(customerString);
                else if (User.IsInRole("Customer"))
                {
                    customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                    customerString = customer.Id;
                }

                if (customer != null)
                {
                    var model = new AppointmentViewModel();
                    model.Customer = customer;
                    model.CustomerId = customerString;
                    model.Date = DateTime.Now.Date;
                    model.TimeslotsList = _appointmentRepository.GetComboTimeslot(0);
                    model.DoctorsList = _appointmentRepository.GetComboDoctor(string.Empty);

                    ViewData["Specialities"] = _specialityRepository.GetFilledSpecialities();

                    return View(model);
                }
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _appointmentRepository.GetByIdWithFKs(id.Value);

            if (appointment == null)
                return NotFound();

            if (User.IsInRole("Customer") && appointment.Customer.Email != User.Identity.Name) //AR
            {
                return NotFound();
            }

            if (appointment.Finished == true)
            {
                return View("AppointmentClosed");
            }

            // Conditional for the converter. Unable to convert a nullable.
            AppointmentViewModel model = new();
            if (appointment.MedicalScreeningId != null)
            {
                model = _converterHelper.ToAppointmentViewModelMS(appointment);
            }
            else
            {
                model = _converterHelper.ToAppointmentViewModel(appointment);
                model.TimeslotsList = _appointmentRepository.GetComboTimeslot(model.SelectedTimeslotId);
                model.DoctorsList = _appointmentRepository.GetComboDoctor(model.SelectedDoctorId);
            }


            model.PrescriptionList = _prescriptionRepository.GetAll().Include("Medicine").Include("Exam").Where(t => t.AppointmentId == model.Id);

            ViewData["Specialities"] = _specialityRepository.GetFilledSpecialities();
            ViewData["Prescriptions"] = _prescriptionRepository.GetByAppointment(appointment.Id);

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Error404", "Error");

            var appointment = await _appointmentRepository.GetByIdWithFKs(id.Value);

            if (appointment == null)
                return NotFound();

            if (User.IsInRole("Customer") && appointment.Customer.Email != User.Identity.Name) //AR
            {
                return NotFound();
            }

            return View(appointment);
        }
        #endregion

        #region POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*string customerString, */AppointmentViewModel model)
        {
            //if (string.IsNullOrEmpty(model.CustomerId))
            //    model.CustomerId = customerString;

            if (ModelState.IsValid)
            {

                var appointment = _converterHelper.ToAppointment(model, true);

                await _appointmentRepository.CreateAsync(appointment);

                //Send Email
                appointment = await _appointmentRepository.GetByIdWithFKs(appointment.Id);
                var response = await _mailHelper.SendAppointmentEmail(appointment, 1, precriptionsList: null); // 1 == Create

                //Prescription
                Random random = new Random();
                var prescriptionNumber = random.Next(1111111, 9999999);

                if (model.MedicineList != null)
                {
                    foreach (var item in model.MedicineList)
                    {
                        var prescription = new Prescription()
                        {
                            MedicineId = int.Parse(item),
                            Number = prescriptionNumber,
                            AppointmentId = appointment.Id,
                        };
                        await _prescriptionRepository.CreateAsync(prescription);

                        _flashMessage.Confirmation("Appointment scheduled");
                    }
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var appt = _converterHelper.ToAppointment(model, false);
                    await _appointmentRepository.UpdateAsync(appt);

                    //Send Email
                    appt = await _appointmentRepository.GetByIdWithFKs(appt.Id);
                    var response = await _mailHelper.SendAppointmentEmail(appt, 2, precriptionsList: null);

                    return RedirectToAction("Details", new { id = appt.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _appointmentRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["Specialities"] = _specialityRepository.GetFilledSpecialities();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SaveNotes(int id, AppointmentViewModel model)
        {
            var appt = await _appointmentRepository.GetByIdAsync(id);

            appt.Notes = model.Notes;

            await _appointmentRepository.UpdateAsync(appt);

            _flashMessage.Info("Changes Saved");

            return RedirectToAction("Edit", "Appointment", new { id = appt.Id });
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Close(int id)
        {
            if (ModelState.IsValid)
            {
                var appointment = await _appointmentRepository.GetByIdWithFKs(id);

                appointment.Finished = true;

                // Email

                if (appointment.Customer.HasInsurance)
                {
                    appointment.Price -= 80;
                }

                await _appointmentRepository.UpdateAsync(appointment);

                //Email

                var response = await _mailHelper.SendAppointmentEmail(appointment, 4, precriptionsList: null); // 1 == Create

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var objSent = await _appointmentRepository.GetAppointmentByIdAsync(id);

            await _appointmentRepository.DeleteAsync(objSent);

            await _mailHelper.SendAppointmentEmail(objSent, 3, null);

            _flashMessage.Confirmation("Appointment canceled");

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region AJAX

        [HttpPost]
        [Route("Appointment/GetTimeslotsAsync")]
        public async Task<JsonResult> GetTimeslotsAsync(DateTime date, int specialityId, int apptId)
        {
            var currentAppt = await _appointmentRepository.GetByIdAsync(apptId);
            var dateResult = await _appointmentRepository.GetTimeslotsFromDateAsync(date.Date, specialityId, currentAppt);

            return Json(dateResult.OrderBy(s => s.Slot));
        }

        [HttpPost]
        [Route("Appointment/GetDoctorsAsync")]
        public async Task<JsonResult> GetDoctorsAsync(int timeslot, int specialityId, DateTime date, int apptId)
        {
            var currentAppt = await _appointmentRepository.GetByIdAsync(apptId);
            var dateResult = await _appointmentRepository.GetDoctorFromTimeslotsAsync(timeslot, specialityId, date, currentAppt);

            return Json(dateResult.OrderBy(s => s.FullName));
        }

        [HttpPost]
        [Authorize]
        [Route("Appointment/GetAppointmentsAsync")]
        public async Task<IActionResult> GetAppointmentsAsync(DateTime date)
        {
            List<Appointment> appts = new();

            if (User.IsInRole("Customer"))
            {
                appts = _appointmentRepository.GetByDateFromCustomer(date.Date, User.Identity.Name).ToList();
            }
            else if (User.IsInRole("Doctor"))
            {
                appts = _appointmentRepository.GetByDateFromDoctor(date.Date, User.Identity.Name).ToList();
            }
            else if (User.IsInRole("Clerk"))
            {
                appts = _appointmentRepository.GetByDate(date).ToList();
            }
            else if (User.IsInRole("Admin"))
            {
                appts = _appointmentRepository.GetByDate(date).ToList();
            }

            ViewData["FilteredDate"] = $"{date:yyyy-MM-dd}";
            return View("Index", appts);
        }

        #endregion
    }
}
