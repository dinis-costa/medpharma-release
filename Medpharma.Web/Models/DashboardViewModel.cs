using Medpharma.Web.Data.Entities;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalClients { get; set; }

        public int TotalClerks { get; set; }

        public int TotalDoctors { get; set; }

        public int TotalOrders { get; set; }

        public int TotalAppointments { get; set; }

        public int TotalPrescriptions { get; set; }

        public int TotalMedicinesSoldWithPrescription { get; set; }

        public int TotalMedicinesSoldWithoutPrescription { get; set; }

        public int TotalMedicinesWithPrescription { get; set; }

        public int TotalMedicinesWithoutPrescription { get; set; }

        public int TotalProducts { get; set; }

        public double TotalAppointmentsRevenue { get; set; }

        public double TotalOrdersRevenue { get; set; }

        public double TotalRevenue => TotalOrdersRevenue + TotalAppointmentsRevenue;

        public List<Appointment> Appointments { get; set; }

        public List<Admission> Admissions { get; set; }

        public List<Order> Orders { get; set; }

        public List<Order> OrdersByCustomer { get; set; }

        public bool WareHouse { get; set; }
    }
}
