using Medpharma.Web.Data.Entities;
using Medpharma.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data
{
    public class SeedDb
    {
        #region Fields & Constructor

        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _userHelper = userHelper;
        }

        #endregion

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            //Roles
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Clerk");
            await _userHelper.CheckRoleAsync("Doctor");
            await _userHelper.CheckRoleAsync("Customer");

            #region Speciality & User

            if (!_context.Specialities.Any())
            {
                AddSpeciality("Cardiology");
                AddSpeciality("Oncology");
                AddSpeciality("Surgery");
                AddSpeciality("Urology");
                AddSpeciality("Neurology");
                AddSpeciality("NeoNatal");
                AddSpeciality("Pediatrics");
                AddSpeciality("Internal medicine");
                AddSpeciality("Orthopedics");
                AddSpeciality("Pathology");
                AddSpeciality("Family Medicine");
                AddSpeciality("Dermatology");
                AddSpeciality("Anesthesiology");
                AddSpeciality("Immunology");
            }

            Customer userA = null;
            Customer userB = null;
            Customer userC = null;

            if (!_context.Users.Any())
            {
                await CreateUser("admin@medpharma.com", "Admin", "User", "000000000", "Admin");
                await CreateClerk("m.sousa@medpharma.com", "Marta", "Sousa", "333666555", "Clerk", false);
                await CreateClerk("f.gilberto@medpharma.com", "Fábio", "Gilberto", "212343212", "Clerk", true);
                await CreateDoctor("m.betencourt@medpharma.com", "Maria", "Betencourt", "565666888", "Doctor", 1);
                await CreateDoctor("a.guimaraes@medpharma.com", "Afonso", "Guimarães", "878565222", "Doctor", 1);
                await CreateDoctor("p.nascimento@medpharma.com", "Paulo", "Nascimento", "156347892", "Doctor", 2);
                await CreateDoctor("r.costa@medpharma.com", "Roberto", "Costa", "298347892", "Doctor", 3);
                await CreateDoctor("a.pinto@medpharma.com", "Acácio", "Pinto", "369258147", "Doctor", 4);
                await CreateDoctor("a.freitas@medpharma.com", "Adriana", "Freitas", "258147369", "Doctor", 5);
                await CreateDoctor("h.silva@medpharma.com", "Helena", "Silva", "789654123", "Doctor", 6);
                await CreateDoctor("h.delgado@medpharma.com", "Henrique", "Delgado", "456123789", "Doctor", 7);
                await CreateDoctor("l.mesquita@medpharma.com", "Luísa", "Mesquita", "123789456", "Doctor", 8);
                await CreateDoctor("l.carvalho@medpharma.com", "Lurdes", "Carvalho", "020898777", "Doctor", 9);
                await CreateDoctor("v.proenca@medpharma.com", "Vitor", "Proença", "197382460", "Doctor", 10);
                await CreateDoctor("z.gomes@medpharma.com", "Zélia", "Gomes", "888777999", "Doctor", 11);
                await CreateDoctor("c.rodrigues@medpharma.com", "Cátia", "Rodrigues", "000123456", "Doctor", 12);
                await CreateDoctor("c.ramalhao@medpharma.com", "Clara", "Ramalhão", "000004567", "Doctor", 13);
                await CreateDoctor("c.ferreira@medpharma.com", "Cristiana", "Ferreira", "741000999", "Doctor", 14);
                userA = await CreateCustomer("customer1@yopmail.com", "Jacinto", "Arménio", "235235235", "Customer", "Rua das Acácias 13, Lisboa", Sex.Male, new DateTime(1947, 12, 31), "912345678", "62", "168", true, "");
                userB = await CreateCustomer("customer2@yopmail.com", "Henriqueta", "Aveiro", "189653277", "Customer", "Rua dos Combatentes 58, Lisboa", Sex.Female, new DateTime(1964, 06, 14), "967654321", "50", "155", true, "");
                userC = await CreateCustomer("customer3@yopmail.com", "António", "Rodrigues", "023456781", "Customer", "Rua Fernando Pessoa 2, Lisboa", Sex.Male, new DateTime(1972, 03, 04), "935577661", "82", "170", false, "");

            }

            await CreateUser("admin@medpharma.com", "Admin", "User", "000000000", "Admin");
            await CreateClerk("m.sousa@medpharma.com", "Marta", "Sousa", "333666555", "Clerk", false);
            await CreateClerk("f.gilberto@medpharma.com", "Fábio", "Gilberto", "212343212", "Clerk", true);
            await CreateDoctor("m.betencourt@medpharma.com", "Maria", "Betencourt", "565666888", "Doctor", 1);
            await CreateDoctor("a.guimaraes@medpharma.com", "Afonso", "Guimarães", "878565222", "Doctor", 1);
            await CreateDoctor("p.nascimento@medpharma.com", "Paulo", "Nascimento", "156347892", "Doctor", 2);
            await CreateDoctor("r.costa@medpharma.com", "Roberto", "Costa", "298347892", "Doctor", 3);
            await CreateDoctor("a.pinto@medpharma.com", "Acácio", "Pinto", "369258147", "Doctor", 4);
            await CreateDoctor("a.freitas@medpharma.com", "Adriana", "Freitas", "258147369", "Doctor", 5);
            await CreateDoctor("h.silva@medpharma.com", "Helena", "Silva", "789654123", "Doctor", 6);
            await CreateDoctor("h.delgado@medpharma.com", "Henrique", "Delgado", "456123789", "Doctor", 7);
            await CreateDoctor("l.mesquita@medpharma.com", "Luísa", "Mesquita", "123789456", "Doctor", 8);
            await CreateDoctor("l.carvalho@medpharma.com", "Lurdes", "Carvalho", "020898777", "Doctor", 9);
            await CreateDoctor("v.proenca@medpharma.com", "Vitor", "Proença", "197382460", "Doctor", 10);
            await CreateDoctor("z.gomes@medpharma.com", "Zélia", "Gomes", "888777999", "Doctor", 11);
            await CreateDoctor("c.rodrigues@medpharma.com", "Cátia", "Rodrigues", "000123456", "Doctor", 12);
            await CreateDoctor("c.ramalhao@medpharma.com", "Clara", "Ramalhão", "000004567", "Doctor", 13);
            await CreateDoctor("c.ferreira@medpharma.com", "Cristiana", "Ferreira", "741000999", "Doctor", 14);
            await CreateCustomer("customer1@yopmail.com", "Jacinto", "Arménio", "235235235", "Customer", "Rua das Acácias 13, Lisboa", Sex.Male, new DateTime(1947, 12, 31), "912345678", "62", "168", true, "");
            await CreateCustomer("customer2@yopmail.com", "Henriqueta", "Aveiro", "189653277", "Customer", "Rua dos Combatentes 58, Lisboa", Sex.Female, new DateTime(1964, 06, 14), "967654321", "50", "155", true, "");
            await CreateCustomer("customer3@yopmail.com", "António", "Rodrigues", "023456781", "Customer", "Rua Fernando Pessoa 2, Lisboa", Sex.Male, new DateTime(1972, 03, 04), "935577661", "82", "170", false, "");

            #endregion

            if (!_context.Priorities.Any())
            {
                AddPriority("Low");
                AddPriority("Medium");
                AddPriority("High");
            }


            if (!_context.Medicines.Any())
            {
                AddMedicine("Paracetamol 500mg", "26x pills", 4.60m, false, 100);
                AddMedicine("Paracetamol 1000mg", "26x pills", 6.00m, false, 100);
                AddMedicine("Iboprufeno 400mg", "12x pills", 3.20m, false, 100);
                AddMedicine("Naproxeno 200mg", "6x pills", 8.00m, true, 100);
                AddMedicine("Agiolax 10mg", "10x pills", 4.10m, true, 100);
                AddMedicine("Betadine 120ml", "Ointment", 5.00m, false, 100);
                AddMedicine("Cetirizina Bluepharma 5ml", "Ointment", 7.16m, true, 100);
                AddMedicine("Daflon 100mg", "14x pills", 3.90m, true, 100);
                AddMedicine("Fitocreme 150ml", "Ointment", 10.00m, true, 0);
                AddMedicine("Buscopan 20mg", "10x pills", 6.20m, false, 100);
                AddMedicine("Cegripe 100mg", "16x pills", 6.60m, false, 100);
                AddMedicine("Meocil 20mg", "Ointment - Eyecare", 5.00m, true, 100);
                AddMedicine("Diprosone 120mg", "Ointment", 4.40m, true, 100);
                AddMedicine("Bepanthene Plus 120mg", "Ointment Gel", 13.10m, false, 100);
                AddMedicine("Nimed 200mg", "20x pills", 8.00m, true, 100);
            }

            if (!_context.Exams.Any())
            {
                AddExams("Bloodwork", 30m);
                AddExams("Bone Marrow", 30m);
                AddExams("Gastric fluid analysis", 50m);
                AddExams("Urine analysis", 30m);
                AddExams("Feces analysis", 30m);
                AddExams("Thyroid", 100m);
                AddExams("Brain Scan", 1500m);
                AddExams("Angiocardiography", 80m);
                AddExams("Endoscopy", 50m);
                AddExams("Magnetic resonance imaging", 500m);
                AddExams("Mammography", 300m);
                AddExams("X-Ray", 65m);
                AddExams("Ultrasound", 120m);
                AddExams("Lung ventilation", 420m);
                AddExams("Biopsy", 75m);
                AddExams("Gynecological - PAP Smear", 50m);
            }

            if (!_context.Timeslots.Any())
            {
                AddTimeslot("09:00 - 09:30");
                AddTimeslot("09:30 - 10:00");
                AddTimeslot("10:00 - 10:30");
                AddTimeslot("10:30 - 11:00");
                AddTimeslot("11:00 - 11:30");
                AddTimeslot("11:30 - 12:00");
                AddTimeslot("12:00 - 12:30");
                AddTimeslot("12:30 - 13:00");
                AddTimeslot("13:00 - 13:30");
                AddTimeslot("13:30 - 14:00");
                AddTimeslot("14:00 - 14:30");
                AddTimeslot("14:30 - 15:00");
                AddTimeslot("15:00 - 15:30");
                AddTimeslot("15:30 - 16:00");
                AddTimeslot("16:00 - 16:30");
                AddTimeslot("16:30 - 17:00");
                AddTimeslot("17:00 - 17:30");
                AddTimeslot("17:30 - 18:00");
            }
        }

        private void AddTimeslot(string slot)
        {
            _context.Timeslots.Add(new Timeslot
            {
                Slot = slot,
            });
            _context.SaveChanges();
        }

        private void AddAdmition(string customerId, string notes)
        {
            _context.Admissions.Add(new Admission
            {
                CustomerId = customerId,
                Notes = notes
            });
            _context.SaveChanges();
        }

        private void AddMedicine(string name, string description, decimal price, bool prescriptionRequired, int stock)
        {
            _context.Medicines.Add(new Medicine
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock,
                NeedsPrescription = prescriptionRequired,
            });
            _context.SaveChanges();
        }

        private void AddExams(string name, decimal price)
        {
            _context.Exams.Add(new Exam
            {
                Name = name,
                Price = price,
            });
            _context.SaveChanges();
        }

        private void AddMedicalScreening(int admissionId, int specialityId, int priorityId, string observations)
        {
            _context.MedicalScreening.Add(new MedicalScreening
            {
                AdmissionId = admissionId,
                SpecialityId = specialityId,
                PriorityId = priorityId,
                Observations = observations
            });
            _context.SaveChanges();
        }

        private void AddSpeciality(string name)
        {

            _context.Specialities.Add(new Speciality
            {
                Name = name,
            });
            _context.SaveChanges();
        }

        private void AddPriority(string name)
        {
            _context.Priorities.Add(new Priority
            {
                Name = name,
            });
            _context.SaveChanges();
        }

        private async Task CreateUser(string email, string fName, string lName, string doc, string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    FirstName = fName,
                    LastName = lName,
                    Document = doc,

                };

                var result = await _userHelper.AddUserAsync(user, "Cinel123!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Unable to generate administrator through seeder.");
                }

                await _userHelper.AddUserToRoleAsync(user, role);
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole3 = await _userHelper.IsUserInRoleAsync(user, role);

            if (!isInRole3)
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }
        }

        private async Task CreateDoctor(string email, string fName, string lName, string doc, string role, int specialityId)
        {
            var doctor = await _userHelper.GetUserByEmailAsync(email);

            if (doctor == null)
            {
                doctor = new Doctor
                {
                    Email = email,
                    UserName = email,
                    FirstName = fName,
                    LastName = lName,
                    Document = doc,
                    SpecialityId = specialityId,
                };

                var result = await _userHelper.AddUserAsync(doctor, "Cinel123!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Unable to generate administrator through seeder.");
                }

                await _userHelper.AddUserToRoleAsync(doctor, role);
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(doctor);
                await _userHelper.ConfirmEmailAsync(doctor, token);
            }

            var isInRole3 = await _userHelper.IsUserInRoleAsync(doctor, role);

            if (!isInRole3)
            {
                await _userHelper.AddUserToRoleAsync(doctor, role);
            }
        }

        private async Task<Customer> CreateCustomer(
            string email, string fName, string lName, string doc, string role,
            string address, Sex sex, DateTime dateOfBirth, string phoneNumber,
            string weight, string height, bool hasInsurance, string medicalInfo)
        {
            var customer = await _userHelper.GetUserByEmailAsync(email);

            if (customer == null)
            {
                customer = new Customer
                {
                    Email = email,
                    UserName = email,
                    FirstName = fName,
                    LastName = lName,
                    Document = doc,
                    Address = address,
                    Sex = sex,
                    DateOfBirth = dateOfBirth,
                    PhoneNumber = phoneNumber,
                    Weight = weight,
                    Height = height,
                    HasInsurance = hasInsurance,
                    MedicalInfo = medicalInfo
                };

                var result = await _userHelper.AddUserAsync(customer, "Cinel123!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Unable to generate administrator through seeder.");
                }

                await _userHelper.AddUserToRoleAsync(customer, role);
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(customer);
                await _userHelper.ConfirmEmailAsync(customer, token);


            }

            var isInRole3 = await _userHelper.IsUserInRoleAsync(customer, role);

            if (!isInRole3)
            {
                await _userHelper.AddUserToRoleAsync(customer, role);
            }

            return customer as Customer;
        }

        private async Task CreateClerk(string email, string fName, string lName, string doc, string role, bool warehouse)
        {
            var clerk = await _userHelper.GetUserByEmailAsync(email);

            if (clerk == null)
            {
                clerk = new Clerk
                {
                    Email = email,
                    UserName = email,
                    FirstName = fName,
                    LastName = lName,
                    Document = doc,
                    WareHouse = warehouse
                };

                var result = await _userHelper.AddUserAsync(clerk, "Cinel123!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Unable to generate administrator through seeder.");
                }

                await _userHelper.AddUserToRoleAsync(clerk, role);
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(clerk);
                await _userHelper.ConfirmEmailAsync(clerk, token);
            }

            var isInRole3 = await _userHelper.IsUserInRoleAsync(clerk, role);

            if (!isInRole3)
            {
                await _userHelper.AddUserToRoleAsync(clerk, role);
            }
        }


    }
}