using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;
        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Appointment> GetByIdToMobile(int id)
        {
            return _context.Appointments
                .Include(app => app.Doctor)
                .Include(app => app.Speciality)
                .Include(app => app.Timeslot)
                .Where(a => a.Id == id)
                .AsQueryable();


        }

        public IQueryable<Appointment> GetAllWithFKs()
        {
            return _context.Appointments.Include("Customer")
                                              .Include("MedicalScreening")
                                              .Include("Timeslot")
                                              .Include("Doctor")
                                              .Include("Speciality")
                                              .Include("Prescriptions")
                                              .Include("Prescriptions.Medicine")
                                              .Include("Prescriptions.Exam");
        }

        //public IQueryable<Appointment> GetByDoctorByDate()
        //{

        //}

        public async Task<Appointment> GetByIdWithFKs(int id)
        {
            return await _context.Appointments.Include("Customer")
                                              .Include("MedicalScreening")
                                              .Include("Timeslot")
                                              .Include("Doctor")
                                              .Include("Speciality")
                                              .Include("Prescriptions")
                                              .Include("Prescriptions.Medicine")
                                              .Include("Prescriptions.Exam")
                                              .Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByCustomer(string email)
        {
            return await _context.Appointments.Include("Customer")
                                              .Include("MedicalScreening")
                                              .Include("Timeslot")
                                              .Include("Doctor")
                                              .Include("Speciality")
                                              .Include("Prescriptions")
                                              .Include("Prescriptions.Medicine")
                                              .Include("Prescriptions.Exam")
                                              .Where(a => a.Customer.Email == email).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctor(string email)
        {
            return await _context.Appointments.Include("Customer")
                                              .Include("MedicalScreening")
                                              .Include("Timeslot")
                                              .Include("Doctor")
                                              .Include("Speciality")
                                              .Include("Prescriptions")
                                              .Include("Prescriptions.Medicine")
                                              .Include("Prescriptions.Exam")
                                              .Where(a => a.Doctor.Email == email).OrderBy(a => a.Date.Date).ThenBy(a => a.TimeslotId).ToListAsync();
        }

        public async Task<Appointment> GetByMedicalScreeningId(int medicalScreeningId)
        {
            return await _context.Appointments.Where(a => a.MedicalScreeningId == medicalScreeningId).FirstOrDefaultAsync();
        }

        public IQueryable<Appointment> GetByDate(DateTime date)
        {
            return _context.Appointments.Include("Customer")
                                        .Include("MedicalScreening")
                                        .Include("Timeslot")
                                        .Include("Doctor")
                                        .Include("Speciality")
                                        .Include("Prescriptions")
                                        .Include("Prescriptions.Medicine")
                                        .Include("Prescriptions.Exam")
                                        .Where(a => a.Date.Date == date.Date);
        }

        public IQueryable<Appointment> GetByDateFromDoctor(DateTime date, string email)
        {
            return this.GetByDate(date).Where(a => a.Doctor.Email == email);
        }
        public IQueryable<Appointment> GetByDateFromCustomer(DateTime date, string email)
        {
            return this.GetByDate(date).Where(a => a.Customer.Email == email);
        }


        #region COMBO
        public IEnumerable<SelectListItem> GetComboDoctor(string doctorId)
        {
            var vets = _context.Doctors.Find(doctorId);
            var list = new List<SelectListItem>();

            if (vets != null)
            {
                list = _context.Doctors.Select(t => new SelectListItem
                {
                    Text = t.FullName,
                    Value = t.Id
                }).OrderBy(t => t.Value).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "Select a doctor...",
                    Value = String.Empty,
                });
            }

            return list;
        }

        public IEnumerable<SelectListItem> GetComboTimeslot(int timeslotId)
        {
            var timeslots = _context.Timeslots.Find(timeslotId);
            var list = new List<SelectListItem>();

            if (timeslots != null)
            {
                list = _context.Timeslots.Select(t => new SelectListItem
                {
                    Text = t.Slot,
                    Value = t.Id.ToString()
                }).OrderBy(t => t.Value).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "Select a timeslot...",
                    Value = "0"
                });
            }

            return list;
        }

        public async Task<List<Timeslot>> GetTimeslotsFromDateAsync(DateTime date, int specialityId, Appointment appt)
        {
            IQueryable<Doctor> activeDoctors = _context.Doctors.Where(d => d.SpecialityId == specialityId).AsQueryable();

            IQueryable<Timeslot> timeslots = _context.Timeslots.Where(slot => slot.Appointments.Where(appt => appt.Date == date).Count() < activeDoctors.Count());

            if (appt != null)
            {
                timeslots = _context.Timeslots.Where(slot => slot.Id == appt.TimeslotId || slot.Appointments.Where(appt => appt.Date == date).Count() < activeDoctors.Count());
            }

            return await timeslots.ToListAsync();
        }

        public async Task<List<Doctor>> GetDoctorFromTimeslotsAsync(int timeslot, int specialityId, DateTime date, Appointment appt)
        {
            var busyDoctors = _context.Appointments.Where(appt => (appt.Date == date) && (appt.TimeslotId == timeslot)).Select(vet => vet.Doctor);

            IQueryable<Doctor> activeDoctors = _context.Doctors.Where(d => d.SpecialityId == specialityId).AsQueryable();

            IQueryable<Doctor> doctorSlots = activeDoctors.Where(vet => !busyDoctors.Contains(vet));

            if (appt != null)
            {
                if (appt.Id != 0) doctorSlots = activeDoctors.Where(d => d.Id == appt.DoctorId || !busyDoctors.Contains(d));
            }

            return await doctorSlots.ToListAsync();
        }

        #endregion

        #region AJAX


        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string userId)
        {
            return await _context.Appointments
                .Include(p => p.Prescriptions)
                .ThenInclude(m => m.Medicine)
                .Include(d => d.Doctor)
                .Include(s => s.Speciality)
                .Where(u => u.CustomerId == userId && u.Prescriptions.Count > 0)
                .OrderBy(a => a.PrescriptionsFilled != 0)
                .ToListAsync();
        }

        public async Task<List<AppointmentRemainingMedicines>> GetAppointmentsRemainingByUserAsync(string userId)
        {
            return await _context.AppointmentRemainingMedicines
                .Include(p => p.PrescriptionRemaining)
                .ThenInclude(m => m.Medicine)
                .Include(d => d.Doctor)
                .Include(s => s.Speciality)
                .Where(u => u.CustomerId == userId)
                .OrderBy(a => a.PrescriptionsFilled != 0)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(c => c.Customer)
                .Include(p => p.Prescriptions)
                .ThenInclude(m => m.Medicine)
                .Include(d => d.Doctor)
                .Include(s => s.Speciality)
                .Include(t => t.Timeslot)
                .FirstOrDefaultAsync(i => i.Id == appointmentId);


        }

        public async Task<AppointmentRemainingMedicines> GetAppointmentRemainingByIdAsync(int appointmentId)
        {
            return await _context.AppointmentRemainingMedicines
                .Include(c => c.Customer)
                .Include(p => p.PrescriptionRemaining)
                .ThenInclude(m => m.Medicine)
                .Include(d => d.Doctor)
                .Include(s => s.Speciality)
                .Include(t => t.Timeslot)
                .FirstOrDefaultAsync(i => i.Id == appointmentId);


        }

        public async Task AddAppointmentRemaining(AppointmentRemainingMedicines entity)
        {
            await _context.AppointmentRemainingMedicines.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAddAppointmentRemaining(AppointmentRemainingMedicines entity)
        {
            _context.AppointmentRemainingMedicines.Update(entity);

            await _context.SaveChangesAsync();
        }

        #endregion

    }
}
