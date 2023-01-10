using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Customer : User
    {
        public Sex Sex { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MMM/yyy}")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Weight { get; set; }

        [Required]
        public string Height { get; set; }

        public bool HasInsurance { get; set; }

        public string? MedicalInfo { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        [MinLength(1, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        public string Address { get; set; }
        public string ImageFullPath => ImageId == Guid.Empty ?
        //$"https://dashpet.blob.core.windows.net/images/noimage.png" :
        //$"https://dashpet.blob.core.windows.net/owners/{ImageId}";
            $"~/images/noimage.png" :
            $"~/images/customer/{ImageId}.jpg";

        public List<CustomerFile> CustomerFiles { get; set; }
    }
    public enum Sex
    {
        Male,
        Female,
        Other,
    }
}
