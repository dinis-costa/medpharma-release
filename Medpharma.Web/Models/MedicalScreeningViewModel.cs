using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class MedicalScreeningViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Priority")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Priority")]
        public int PId { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }

        [Display(Name = "Speciality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Speciality")]
        public int SId { get; set; }
        public IEnumerable<SelectListItem> Specialities { get; set; }

        public int AdmissionId { get; set; }

        public Admission Admission { get; set; }

        public int SpecialityId { get; set; }

        public Speciality Speciality { get; set; }

        public int PriorityId { get; set; }

        public Priority Priority { get; set; }

        public string Observations { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:HH:mm}")]
        public DateTime Time { get; set; } = DateTime.Now;








    }
}
