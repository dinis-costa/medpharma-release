using System;

namespace Medpharma.Web.Data.Entities
{
    public class PrescriptionRemaining
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int Quantity { get; set; }

        public virtual Medicine Medicine { get; set; }
        public int MedicineId { get; set; }

        public virtual Appointment Appointment { get; set; }
        public int AppointmentId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public decimal Total => Quantity * Medicine.Price;
    }
}
