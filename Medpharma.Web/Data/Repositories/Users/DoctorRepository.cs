using Medpharma.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Medpharma.Web.Data.Repositories.Users
{
    public class DoctorRepository : UserRepository<Doctor>, IDoctorRepository
    {
        private readonly DataContext _context;

        public DoctorRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Doctor> GetAll()
        {
            return _context.Doctors.Include("Speciality").AsNoTracking();
        }

        public IQueryable<Doctor> GetTest(string id)
        {
            return _context.Doctors.Include("Speciality").Where(d => d.Id == id);
        }

        public Doctor GetById(string id)
        {
            return _context.Doctors.Where(d => d.Id == id).Include("Speciality").FirstOrDefault();
        }


        //public async Task<Doctor> GetByIdAsync(string id)
        //{
        //    return await _context.Doctors.Include("Speciality").FirstOrDefaultAsync(d => d.Id == id);
        //}
    }
}