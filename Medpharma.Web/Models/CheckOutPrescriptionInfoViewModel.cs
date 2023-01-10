using Medpharma.Web.Data.Entities;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class CheckOutPrescriptionInfoViewModel
    {
        public Appointment Appointment { get; set; }

        public List<Prescription> ListMedicineStockOut { get; set; }

        public List<Prescription> ListMedicineStockIn { get; set; }
    }
}
