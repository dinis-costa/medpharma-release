using Medpharma.Web.Data.Entities;
using Medpharma.Web.Models;
using System;

namespace Medpharma.Web.Helpers
{
    public interface IConverterHelper
    {
        public Admission ToAdmission(AdmissionViewModel model, bool isNew);
        public Customer ToCustomer(CustomerViewModel model, Guid imageId, bool isNew);
        public CustomerViewModel ToCustomerViewModel(Customer customer);


        MedicalScreening ToMedicalScreening(MedicalScreeningViewModel model, bool isNew);
        MedicalScreeningViewModel ToMedicalScreeningViewModel(MedicalScreening model, bool isNew);

        public Doctor ToDoctor(DoctorViewModel model, Guid imageId, bool isNew);
        public DoctorViewModel ToDoctorViewModel(Doctor customer);

        public Clerk ToClerk(ClerkViewModel model, Guid imageId, bool isNew);
        public ClerkViewModel ToClerkViewModel(Clerk customer);

        public Medicine ToMedicine(MedicineViewModel model, Guid imageId, bool isNew);
        public MedicineViewModel ToMedicineViewModel(Medicine medicine);

        AppointmentViewModel ToAppointmentViewModel(Appointment appt);
        Appointment ToAppointment(AppointmentViewModel model, bool isNew);

        Prescription ToPrescription(PrescriptionViewModel model, bool isNew);
        PrescriptionViewModel ToPrescriptionViewModel(Prescription prescription);
        Appointment ToAppointmentModel(AppointmentRemainingMedicines model);
        AppointmentViewModel ToAppointmentViewModelMS(Appointment appt);
    }
}
