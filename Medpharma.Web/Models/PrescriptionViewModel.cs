using Medpharma.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class PrescriptionViewModel : Prescription
    {
        [Range(1, int.MaxValue)]
        public string[] MedicineList { get; set; }

        public IEnumerable<Medicine> MedicinesList { get; set; }

        public string[] ExamList { get; set; }

        public IEnumerable<Exam> ExamsList { get; set; }
    }
}
