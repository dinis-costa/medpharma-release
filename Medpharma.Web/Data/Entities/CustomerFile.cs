using System;

namespace Medpharma.Web.Data.Entities
{
    // Customer file can be related with regular appointment or remaining appointment
    public class CustomerFile : IEntity
    {
        public CustomerFile()
        {
            TimeStamp = DateTime.Now;
        }
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual Appointment Appointment { get; set; }
        public int? AppointmentId { get; set; }

        public virtual AppointmentRemainingMedicines AppointmentRemainingMedicines { get; set; }
        public int? AppointmentRemainingMedicinesId { get; set; }

        public virtual Order Order { get; set; }
        public int? OrderId { get; set; }

        public DateTime TimeStamp { get; set; }

        public Guid FileId { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public byte Type { get; set; } = 1; // 1 == Uploaded By Customer | 2 == Invoice | 3 == Prescription | 4 == Exam | 5 == Shop Invoice


    }
}
