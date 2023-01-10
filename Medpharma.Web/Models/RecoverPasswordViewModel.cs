using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
