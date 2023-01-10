using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> GetByMedicalScreeningId(int medicalScreeningId);
        Task<Appointment> GetByIdWithFKs(int id); IEnumerable<SelectListItem> GetComboDoctor(string doctorId);
        IEnumerable<SelectListItem> GetComboTimeslot(int timeslotId);
        Task<List<Timeslot>> GetTimeslotsFromDateAsync(DateTime date, int specialityId, Appointment appt);
        Task<List<Doctor>> GetDoctorFromTimeslotsAsync(int timeslot, int specialityId, DateTime date, Appointment appt);
        IQueryable<Appointment> GetByDate(DateTime date);
        Task<List<Appointment>> GetAppointmentsByUserAsync(string userId);
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
        Task AddAppointmentRemaining(AppointmentRemainingMedicines entity);
        Task<List<AppointmentRemainingMedicines>> GetAppointmentsRemainingByUserAsync(string userId);
        Task<AppointmentRemainingMedicines> GetAppointmentRemainingByIdAsync(int appointmentId);
        Task UpdateAddAppointmentRemaining(AppointmentRemainingMedicines entity);
        Task<List<Appointment>> GetAppointmentsByCustomer(string id);
        Task<List<Appointment>> GetAppointmentsByDoctor(string email);
        IQueryable<Appointment> GetAllWithFKs();
        IQueryable<Appointment> GetByDateFromDoctor(DateTime date, string email);
        IQueryable<Appointment> GetByDateFromCustomer(DateTime date, string email);
        IQueryable<Appointment> GetByIdToMobile(int id);
    }
}

