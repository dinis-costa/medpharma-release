using Medpharma.Web.Data.Entities;
using Medpharma.Web.Models;
using System;

namespace Medpharma.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {

        public Admission ToAdmission(AdmissionViewModel model, bool isNew)
        {
            return new Admission
            {
                Id = isNew ? 0 : model.Id,
                CustomerId = model.CustomerId,
                Notes = model.Notes,
            };
        }

        public Customer ToCustomer(CustomerViewModel model, Guid imageId, bool isNew)
        {
            return new Customer
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Document = model.Document,
                Address = model.Address,
                Email = model.Email,
                ImageId = imageId,
                UserName = model.Email,
                Sex = model.Sex,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth.Date,
                Weight = model.Weight,
                Height = model.Height,
                HasInsurance = model.HasInsurance,
                MedicalInfo = model.MedicalInfo,
            };
        }


        public CustomerViewModel ToCustomerViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Email = customer.Email,
                UserName = customer.UserName,
                Document = customer.Document,
                ImageId = customer.ImageId,
                Sex = customer.Sex,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth.Date,
                Weight = customer.Weight,
                Height = customer.Height,
                HasInsurance = customer.HasInsurance,
                MedicalInfo = customer.MedicalInfo,
            };
        }

        public Doctor ToDoctor(DoctorViewModel model, Guid imageId, bool isNew)
        {
            return new Doctor
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Document = model.Document,
                Email = model.Email,
                ImageId = imageId,
                UserName = model.Email,
                SpecialityId = model.SpecialityId,
            };
        }

        public DoctorViewModel ToDoctorViewModel(Doctor item)
        {
            return new DoctorViewModel
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                UserName = item.UserName,
                Document = item.Document,
                ImageId = item.ImageId,
                SpecialityId = item.SpecialityId,
            };
        }

        public Clerk ToClerk(ClerkViewModel model, Guid imageId, bool isNew)
        {
            return new Clerk
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Document = model.Document,
                Email = model.Email,
                ImageId = imageId,
                UserName = model.Email,
                WareHouse = model.WareHouse,
            };
        }

        public ClerkViewModel ToClerkViewModel(Clerk item)
        {
            return new ClerkViewModel
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                UserName = item.UserName,
                Document = item.Document,
                ImageId = item.ImageId,
                WareHouse = item.WareHouse,
            };
        }

        public Medicine ToMedicine(MedicineViewModel model, Guid imageId, bool isNew)
        {
            return new Medicine
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                NeedsPrescription = model.NeedsPrescription,
                Price = model.Price,
                Stock = model.Stock,
                ImageId = imageId,
            };
        }

        public MedicineViewModel ToMedicineViewModel(Medicine item)
        {
            return new MedicineViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                NeedsPrescription = item.NeedsPrescription,
                Price = item.Price,
                Stock = item.Stock,
                ImageId = item.ImageId,
            };
        }

        public MedicalScreening ToMedicalScreening(MedicalScreeningViewModel model, bool isNew)
        {

            return new MedicalScreening
            {
                Id = (int)(isNew ? 0 : model.Id),
                AdmissionId = model.AdmissionId,
                SpecialityId = model.SId,
                PriorityId = model.PId,
                Observations = model.Observations
            };
        }

        public MedicalScreeningViewModel ToMedicalScreeningViewModel(MedicalScreening model, bool isNew)
        {
            return new MedicalScreeningViewModel
            {
                Id = (int)(isNew ? 0 : model.Id),
                Admission = model.Admission,
                PId = model.PriorityId,
                SId = model.SpecialityId,
                Speciality = model.Speciality,
                Priority = model.Priority,
                Observations = model.Observations
            };
        }

        public AppointmentViewModel ToAppointmentViewModel(Appointment appt)
        {
            return new AppointmentViewModel
            {
                Id = appt.Id,
                Date = appt.Date,
                Notes = appt.Notes,
                Status = appt.Status,
                MedicalScreening = appt.MedicalScreening,
                MedicalScreeningId = appt.MedicalScreeningId,
                SelectedDoctorId = appt.DoctorId,
                Doctor = appt.Doctor,
                Speciality = appt.Speciality,
                SpecialityId = appt.SpecialityId,
                Customer = appt.Customer,
                CustomerId = appt.CustomerId,
                SelectedTimeslotId = appt.TimeslotId.Value,
            };
        }

        public AppointmentViewModel ToAppointmentViewModelMS(Appointment appt)
        {
            return new AppointmentViewModel
            {
                Id = appt.Id,
                Date = appt.Date,
                Notes = appt.Notes,
                Status = appt.Status,
                MedicalScreening = appt.MedicalScreening,
                MedicalScreeningId = appt.MedicalScreeningId,
                SelectedDoctorId = appt.DoctorId,
                Doctor = appt.Doctor,
                Speciality = appt.Speciality,
                SpecialityId = appt.SpecialityId,
                Customer = appt.Customer,
                CustomerId = appt.CustomerId,
            };
        }

        public Appointment ToAppointment(AppointmentViewModel model, bool isNew)
        {
            return new Appointment
            {
                Id = isNew ? 0 : model.Id,
                Date = model.Date.Date,
                Notes = model.Notes,
                Status = model.Status,
                Doctor = model.Doctor,
                DoctorId = model.SelectedDoctorId,
                Timeslot = model.Timeslot,
                TimeslotId = model.SelectedTimeslotId,
                Customer = model.Customer,
                CustomerId = model.CustomerId,
                Speciality = model.Speciality,
                SpecialityId = model.SpecialityId,
                MedicalScreeningId = model.MedicalScreeningId,
                MedicalScreening = model.MedicalScreening,
            };
        }

        public Prescription ToPrescription(PrescriptionViewModel model, bool isNew)
        {
            return new Prescription
            {
                Id = isNew ? 0 : model.Id,
                Number = model.Number,
                Observations = model.Observations,
                Appointment = model.Appointment,
                AppointmentId = model.AppointmentId,
                Quantity = model.Quantity,
                ExpirationDate = model.ExpirationDate,
                ExamId = model.ExamId,
                Exam = model.Exam,
                MedicineId = model.MedicineId,
                Medicine = model.Medicine,
            };
        }

        public PrescriptionViewModel ToPrescriptionViewModel(Prescription prescription)
        {
            return new PrescriptionViewModel
            {
                Id = prescription.Id,
                Number = prescription.Number,
                Observations = prescription.Observations,
                Appointment = prescription.Appointment,
                AppointmentId = prescription.AppointmentId,
                Quantity = prescription.Quantity,
                ExpirationDate = prescription.ExpirationDate,
                ExamId = prescription.ExamId,
                Exam = prescription.Exam,
                MedicineId = prescription.MedicineId,
                Medicine = prescription.Medicine,
            };
        }

        public Appointment ToAppointmentModel(AppointmentRemainingMedicines model)
        {
            return new Appointment
            {
                Id = model.Id,
                Date = model.Date,
                IsRemaining = 1,
                Prescriptions = model.PrescriptionRemaining,
                Doctor = model.Doctor,
                Speciality = model.Speciality,
                Timeslot = model.Timeslot,
                Customer = model.Customer

            };
        }



    }
}
