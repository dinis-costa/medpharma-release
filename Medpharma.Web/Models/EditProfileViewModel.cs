using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required]
        //[Display(Name = "Citizen Card Number")]
        //public string Document { get; set; }

        //[Required]
        //public string Address { get; set; }

        //[Required]
        //[Display(Name = "Phone Number")]
        //[StringLength(9), MinLength(9)]
        //public string PhoneNumber { get; set; }
    }
}
