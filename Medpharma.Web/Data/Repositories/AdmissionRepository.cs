using Medpharma.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public class AdmissionRepository : GenericRepository<Admission>, IAdmissionRepository
    {
        private readonly DataContext _context;

        public AdmissionRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Admission> GetByIdWithInfo(int id)
        {
            return await _context.Admissions
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(ad => ad.Id == id);
        }
    }
}
