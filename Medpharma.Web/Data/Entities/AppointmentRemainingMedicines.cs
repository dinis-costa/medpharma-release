using System;
using System.Collections.Generic;

namespace Medpharma.Web.Data.Entities
{
    public class AppointmentRemainingMedicines : IEntity
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public bool Status { get; set; }

        public int IsRemaining { get; set; }

        public List<Prescription> PrescriptionRemaining { get; set; }

        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public virtual Speciality Speciality { get; set; }
        public int SpecialityId { get; set; }

        public virtual Timeslot Timeslot { get; set; }
        public int TimeslotId { get; set; }

        public virtual Customer Customer { get; set; }
        public string CustomerId { get; set; }

        public int PrescriptionsFilled { get; set; } = 0;
    }
}
