using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Must be a minimum of 8 characters in lenght and include: uppercase, lowercase, number, special.")]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]

        public string ConfirmPassword { get; set; }


        [Required]
        public string Token { get; set; }
    }
}
