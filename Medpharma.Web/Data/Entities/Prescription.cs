using System;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Prescription : IEntity
    {
        public int Id { get; set; }

        public int? Number { get; set; }

        public string? Observations { get; set; }

        public virtual Exam? Exam { get; set; }
        public int? ExamId { get; set; }

        public int Quantity { get; set; } = 1;

        public virtual Medicine? Medicine { get; set; }
        public int? MedicineId { get; set; }

        public virtual Appointment? Appointment { get; set; }
        public int? AppointmentId { get; set; }

        [Display(Name = "Expiration Date")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MMM/yyy}")]
        public DateTime ExpirationDate { get; set; }

        public decimal Total => Medicine == null ? 0m : Quantity * Medicine.Price;
    }
}
