using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class AppointmentViewModel : Appointment
    {
        public IEnumerable<SelectListItem>? TimeslotsList { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The field Timeslot cannot be empty.")]
        [Display(Name = "Timeslot")]
        public int SelectedTimeslotId { get; set; }

        public IEnumerable<SelectListItem>? DoctorsList { get; set; }

        [Required]
        [Display(Name = "Doctor")]
        public string SelectedDoctorId { get; set; }

        public IEnumerable<Prescription>? PrescriptionList { get; set; }

        public string[] MedicineList { get; set; }
    }
}
