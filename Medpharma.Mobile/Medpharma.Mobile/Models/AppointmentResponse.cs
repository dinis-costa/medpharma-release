using System;
using System.Collections.Generic;
using System.Text;

namespace Medpharma.Mobile.Models
{
    public class AppointmentResponse
    {
            public int appId { get; set; }
            public string doctor { get; set; }
            public string speciality { get; set; }
            public DateTime date { get; set; }
            public string timeSlot { get; set; }
            public bool status { get; set; }
    }
}
