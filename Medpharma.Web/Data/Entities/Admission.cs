using System;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Admission : IEntity
    {
        public Admission()
        {
            AdmissionTime = DateTime.Now;
        }

        public int Id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:HH:mm}")]
        public DateTime AdmissionTime { get; set; }
        [Required]
        public string Notes { get; set; }

        //FK
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int Registered { get; set; } = 0;
    }
}
