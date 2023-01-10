using Medpharma.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public class MedicalScreeningRepository : GenericRepository<MedicalScreening>, IMedicalScreeningRepository
    {
        private readonly DataContext _context;

        public MedicalScreeningRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<MedicalScreening> EntityWithFKs()
        {
            return _context.MedicalScreening.Include("Admission.Customer")
                                            .Include("Speciality")
                                            .Include("Priority");
        }

        public IQueryable<MedicalScreening> GetBySpeciality(int specialityId)
        {
            return EntityWithFKs().Where(ms => ms.SpecialityId == specialityId);
        }


        public List<MedicalScreening> GetAllWithInfo()
        {
            return _context.MedicalScreening
                .Include(c => c.Admission)
                .ThenInclude(u => u.Customer)
                .Include(s => s.Speciality)
                .Include(p => p.Priority)
                .ToList();
        }

        public async Task<MedicalScreening> GetByIdWithInfo(int id)
        {
            return await _context.MedicalScreening
                .Include(c => c.Admission)
                .ThenInclude(u => u.Customer)
                .Include(s => s.Speciality)
                .Include(p => p.Priority)
                .FirstOrDefaultAsync(ms => ms.Id == id);
        }

    }

}
