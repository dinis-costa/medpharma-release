using System.Collections.Generic;

namespace Medpharma.Web.Data.Entities
{
    public class Timeslot : IEntity
    {
        public int Id { get; set; }

        public string Slot { get; set; }

        public List<Appointment> Appointments { get; set; }
    }
}
