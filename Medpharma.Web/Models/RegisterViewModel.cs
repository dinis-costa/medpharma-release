using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Medpharma.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Citizen Card Number")]
        [MaxLength(9, ErrorMessage = "Invalid number")]
        [MinLength(9, ErrorMessage = "Invalid number")]
        public string Document { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [StringLength(9), MinLength(9)]
        public string PhoneNumber { get; set; }


        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Must be a minimum of 8 characters in lenght and include: uppercase, lowercase, number, special.")]
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string Confirm { get; set; }
    }
}
