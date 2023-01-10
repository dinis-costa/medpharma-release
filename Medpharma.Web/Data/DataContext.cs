using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Medpharma.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Speciality> Specialities { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Clerk> Clerks { get; set; }

        public DbSet<Priority> Priorities { get; set; }

        public DbSet<MedicalScreening> MedicalScreening { get; set; }

        public DbSet<Medicine> Medicines { get; set; }

        public DbSet<Exam> Exams { get; set; }

        public DbSet<Timeslot> Timeslots { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }


        public DbSet<OrderDetailTemp> OrderDetailTemp { get; set; }

        public DbSet<CustomerFile> CustomerFiles { get; set; }

        public DbSet<Admission> Admissions { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<AppointmentRemainingMedicines> AppointmentRemainingMedicines { get; set; }

        public DbSet<PrescriptionRemaining> PrescriptionRemaining { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicine>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Exam>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetailTemp>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");



            modelBuilder.Entity<User>().HasIndex(e => e.Document).IsUnique();




            base.OnModelCreating(modelBuilder);
        }
    }
}