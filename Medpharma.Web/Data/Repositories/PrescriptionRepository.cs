using Medpharma.Web.Data.Entities;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        private readonly DataContext _context;

        public PrescriptionRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetByAppointment(int apptId)
        {
            return _context.Prescriptions.Where(p => p.AppointmentId == apptId);
        }
    }
}
