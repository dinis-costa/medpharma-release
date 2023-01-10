using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class User : IdentityUser, IUser
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        [MinLength(1, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        [MinLength(1, ErrorMessage = "The field {0} only can contain {1} characters lenght.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(9, ErrorMessage = "Invalid number")]
        [MinLength(9, ErrorMessage = "Invalid number")]
        [Display(Name = "Citizen Card")]
        public string Document { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public Guid ImageId { get; set; }
    }
}
