using Medpharma.Web.Data.Entities;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class FillPrescriptionViewModel
    {
        public int AppointmentId { get; set; }

        public List<Appointment> Appointments { get; set; }
        public List<AppointmentRemainingMedicines> AppointmentsRemaining { get; set; }


    }
}
