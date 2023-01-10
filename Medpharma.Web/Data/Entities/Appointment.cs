using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMMM/yyy}")]
        public DateTime Date { get; set; }

        public string Notes { get; set; }

        public bool Status { get; set; }

        public List<Prescription>? Prescriptions { get; set; }

        public int? MedicalScreeningId { get; set; }
        public virtual MedicalScreening? MedicalScreening { get; set; }

        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [Display(Name = "Speciality")]
        public int SpecialityId { get; set; }
        public virtual Speciality Speciality { get; set; }

        public virtual Customer Customer { get; set; }
        public string CustomerId { get; set; }

        public int? TimeslotId { get; set; }
        public Timeslot Timeslot { get; set; }

        public int PrescriptionsFilled { get; set; } = 0;

        public int IsRemaining { get; set; } = 0;

        public bool Finished { get; set; }

        public bool IsPaid { get; set; }

        public double Price { get; set; } = 100;
    }
}
