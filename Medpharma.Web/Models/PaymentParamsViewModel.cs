using Medpharma.Web.Data.Entities;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class PaymentParamsViewModel
    {
        public int? Origin { get; set; }

        public int? AppointmentId { get; set; }

        public List<PrescriptionRemaining> ListMedicineStockOut { get; set; } = new List<PrescriptionRemaining>();

        public List<Prescription> ListMedicineStockIn { get; set; } = new List<Prescription>();
    }
}
