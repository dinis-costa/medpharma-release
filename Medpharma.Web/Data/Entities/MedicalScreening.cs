using System;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class MedicalScreening : IEntity
    {
        public int Id { get; set; }

        public int AdmissionId { get; set; }

        public Admission Admission { get; set; }

        public int SpecialityId { get; set; }

        public Speciality Speciality { get; set; }
        public int PriorityId { get; set; }
        public bool IsAccepted { get; set; } // Was "pulled" by a doctor.
        public Priority Priority { get; set; }
        public string Observations { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:HH:mm}")]
        public DateTime Time { get; set; } = DateTime.Now;


    }
}
