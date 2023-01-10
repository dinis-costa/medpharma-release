using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class StripeFormViewModel
    {
        public int? Origin { get; set; }
        public int? AppointmentId { get; set; }
        public string? TotalPayment { get; set; }

        [Required(ErrorMessage = "card number is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "card number must be a number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "exp. month is required")]
        [RegularExpression("([0-9]*)", ErrorMessage = "exp. month must be a number")]
        [StringLength(2, ErrorMessage = "maximum 2 numbers")]
        public string ExpMonth { get; set; }

        [Required(ErrorMessage = "exp. year is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "year must be a number")]
        [StringLength(4, ErrorMessage = "maximum 4 numbers")]
        public string ExpYear { get; set; }

        [Required(ErrorMessage = "cvv is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "cvv must be a number")]
        [StringLength(3, ErrorMessage = "maximum 3 numbers")]
        public string Cvv { get; set; }

        [Required(ErrorMessage = "customer name is required")]
        public string CustomerName { get; set; }
    }
}
