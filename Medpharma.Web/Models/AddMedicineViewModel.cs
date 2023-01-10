using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class AddMedicineViewModel
    {

        [Range(1, int.MaxValue, ErrorMessage = "You must select a medicine")]
        public int MedicineId { get; set; }


        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantity must be a positive number")]
        public int Quantity { get; set; }
    }
}
