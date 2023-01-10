using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class StripeController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICheckoutHelper _checkoutHelper;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IStripeHelper _stripeHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public StripeController(IUserHelper userHelper,
            IOrderRepository orderRepository,
            IAppointmentRepository appointmentRepository,
            ICheckoutHelper checkoutHelper,
            IPrescriptionRepository prescriptionRepository,
            IConverterHelper converterHelper,
            IStripeHelper stripeHelper,
             IConfiguration configuration,
             IFlashMessage flashMessage,
             IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _orderRepository = orderRepository;
            _appointmentRepository = appointmentRepository;
            _checkoutHelper = checkoutHelper;
            _prescriptionRepository = prescriptionRepository;
            _converterHelper = converterHelper;
            _stripeHelper = stripeHelper;
            _configuration = configuration;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
        }


        public IActionResult Index(int? appointmentId, string? totalPayment, int? origin)
        {
            if (appointmentId == null || totalPayment == null || origin == null)
                return NotFound();

            var model = new StripeFormViewModel
            {
                Origin = origin.Value,
                AppointmentId = appointmentId.Value,
                TotalPayment = totalPayment
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(StripeFormViewModel model1)
        {

            var appointmentId = model1.AppointmentId;
            var totalPayment = (int)(decimal.Parse(model1.TotalPayment) * 100);
            var origin = model1.Origin;
            var cardNumber = model1.CardNumber;
            var expMonth = model1.ExpMonth;
            var expYear = model1.ExpYear;
            var cvv = model1.Cvv;
            var customerName = model1.CustomerName;

            var customer = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (customer == null)
                return NotFound();

            switch (origin)
            {
                case 1: // Origin: regular prescription
                    Response paymentResponsePres = await _stripeHelper.PayAsync(cardNumber, expMonth, expYear, cvv, totalPayment);

                    if (paymentResponsePres.IsSuccess)
                    {
                        var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId.Value);

                        if (appointment == null)
                            return NotFound();


                        var model = await _checkoutHelper.GetPrescriptionRemainingAsync(appointment);

                        if (await _orderRepository.ConfirmOrderPrescriptionAsync(customer.Id, model.ListMedicineStockIn))
                        {
                            appointment.PrescriptionsFilled = 1;

                            await _appointmentRepository.UpdateAsync(appointment);


                            if (model.ListMedicineStockOut.Count > 0)
                            {
                                var remainingAppointment = new AppointmentRemainingMedicines
                                {
                                    Date = appointment.Date,
                                    Status = appointment.Status,
                                    IsRemaining = 1,
                                    PrescriptionRemaining = model.ListMedicineStockOut,
                                    DoctorId = appointment.DoctorId,
                                    SpecialityId = appointment.SpecialityId,
                                    CustomerId = appointment.CustomerId,
                                    PrescriptionsFilled = 0,
                                    Timeslot = appointment.Timeslot
                                };

                                foreach (var item in model.ListMedicineStockOut)
                                {
                                    item.Appointment = null;

                                    await _prescriptionRepository.UpdateAsync(item);

                                }

                                await _appointmentRepository.AddAppointmentRemaining(remainingAppointment);


                            }


                            await _mailHelper.SendAppointmentEmail(appointment, 5, model.ListMedicineStockIn);
                            //await _mailHelper.GenerateMedicalPrescriptionRemainig(appointment, model.ListMedicineStockOut);

                            return RedirectToAction("SuccessMessage");
                        }
                    }
                    else if (!paymentResponsePres.IsSuccess)
                    {
                        _flashMessage.Danger(paymentResponsePres.Message);
                        return RedirectToAction("Index", "Stripe", new { appointmentId = model1.AppointmentId, totalPayment = model1.TotalPayment, origin = model1.Origin });
                    }

                    break;

                case 2: // Origin: Cart
                    Response paymentResponseCart = await _stripeHelper.PayAsync(cardNumber, expMonth, expYear, cvv, totalPayment);

                    if (paymentResponseCart.IsSuccess)
                    {

                        var order = await _orderRepository.ConfirmOrderAsync(customer.Id);

                        if (order != null)
                            await _mailHelper.SendCartOrderEmail(order);

                        return RedirectToAction("SuccessMessage");
                    }
                    else if (!paymentResponseCart.IsSuccess)
                    {
                        _flashMessage.Danger(paymentResponseCart.Message);
                        return RedirectToAction("Index", "Stripe", new { appointmentId = model1.AppointmentId, totalPayment = model1.TotalPayment, origin = model1.Origin });
                    }

                    break;

                case 3: // Origin: prescription remaining
                    Response paymentResponsePresRem = await _stripeHelper.PayAsync(cardNumber, expMonth, expYear, cvv, totalPayment);

                    if (paymentResponsePresRem.IsSuccess)
                    {
                        var appointmentRem = await _appointmentRepository.GetAppointmentRemainingByIdAsync(appointmentId.Value);

                        if (appointmentRem == null)
                            return NotFound();

                        var appointmentModel = _converterHelper.ToAppointmentModel(appointmentRem);

                        var modelRem = await _checkoutHelper.GetPrescriptionRemainingAsync(appointmentModel);

                        await _mailHelper.SendAppointmentEmail(appointmentModel, 5, modelRem.ListMedicineStockIn);
                        //await _mailHelper.GenerateMedicalPrescriptionRemainig(appointmentModel, modelRem.ListMedicineStockOut);

                        if (await _orderRepository.ConfirmOrderPrescriptionAsync(customer.Id, modelRem.ListMedicineStockIn))
                        {
                            appointmentRem.PrescriptionsFilled = 1;

                            await _appointmentRepository.UpdateAddAppointmentRemaining(appointmentRem);

                            if (modelRem.ListMedicineStockOut.Count > 0)
                            {
                                var remainingAppointment = new AppointmentRemainingMedicines
                                {

                                    Date = appointmentRem.Date,
                                    Status = appointmentRem.Status,
                                    IsRemaining = 1,
                                    PrescriptionRemaining = modelRem.ListMedicineStockOut,
                                    DoctorId = appointmentRem.DoctorId,
                                    SpecialityId = appointmentRem.SpecialityId,
                                    CustomerId = appointmentRem.CustomerId,
                                    PrescriptionsFilled = 0,
                                    Timeslot = appointmentRem.Timeslot

                                };

                                foreach (var item in modelRem.ListMedicineStockOut)
                                {
                                    item.Appointment = null;

                                    await _prescriptionRepository.UpdateAsync(item);

                                }

                                await _appointmentRepository.AddAppointmentRemaining(remainingAppointment);
                            }
                        }

                        return RedirectToAction("SuccessMessage");
                    }
                    else if (!paymentResponsePresRem.IsSuccess)
                    {
                        _flashMessage.Danger(paymentResponsePresRem.Message);
                        return RedirectToAction("Index", "Stripe", new { appointmentId = model1.AppointmentId, totalPayment = model1.TotalPayment, origin = model1.Origin });
                    }
                    break;

                case 4: // Origin: Appointment
                    Response paymentResponseAppt = await _stripeHelper.PayAsync(cardNumber, expMonth, expYear, cvv, totalPayment);

                    if (paymentResponseAppt.IsSuccess)
                    {
                        var appointment = await _appointmentRepository.GetByIdAsync(model1.AppointmentId.Value);

                        appointment.IsPaid = true;

                        await _appointmentRepository.UpdateAsync(appointment);

                        return RedirectToAction("SuccessMessage");
                    }
                    else if (!paymentResponseAppt.IsSuccess)
                    {
                        _flashMessage.Danger(paymentResponseAppt.Message);
                        return RedirectToAction("Index", "Stripe", new { appointmentId = model1.AppointmentId, totalPayment = model1.TotalPayment, origin = model1.Origin });
                    }
                    break;
                
                default:
                    return NotFound();
            }
            return BadRequest();
        }

        public IActionResult SuccessMessage()
        {
            return View();
        }

    }
}
