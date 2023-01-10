using Medpharma.Mobile.Models;
using Medpharma.Web.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Medpharma.Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsMobileController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentsMobileController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        [HttpGet]
        [Route("getUserAppointments")]
        public IActionResult GetUserAppointments(AuthIdentity model)
        {
            var app = _appointmentRepository.GetAll()
                .Include(app => app.Doctor)
                .Include(app => app.Customer)
                .Include(app => app.Speciality)
                .Include(app => app.Timeslot)
                .Where(app => app.Customer.Email == model.Email)
                .Select(app => new
                {
                    AppId = app.Id,
                    Doctor = app.Doctor.FullName,
                    Speciality = app.Speciality.Name,
                    Date = app.Date,
                    TimeSlot = app.Timeslot.Slot,
                    Status = app.Status,
                });

            return Ok(app);
        }

        [HttpGet]
        [Route("getUserAppointmentById/{appId}")]
        public IActionResult GetUserAppointments(int? appId)
        {
            var app = _appointmentRepository.GetByIdToMobile(appId.Value)
                .Select(app => new
                {
                    AppId = app.Id,
                    Doctor = app.Doctor.FullName,
                    Speciality = app.Speciality.Name,
                    Date = app.Date,
                    TimeSlot = app.Timeslot.Slot
                });

            return Ok(app);
        }
    }
}
