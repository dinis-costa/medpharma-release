using Medpharma.Web.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body);
        Task<Response> SendAppointmentEmail(Appointment appointment, byte type, List<Prescription>? precriptionsList);
        Task<string> GenerateInvoice(Appointment appt);
        Task<string> GenerateMedicalPrescription(Appointment appt);
        Task<string> GeneratePrescriptionsInvoice(Appointment appt, List<Prescription> prescriptionsList);
        Task<Response> SendCartOrderEmail(Order order);
        //Task<string> GenerateMedicalPrescriptionRemainig(Appointment appt, List<Prescription> prescriptionRamining);
    }
}
