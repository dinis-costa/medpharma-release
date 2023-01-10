using Medpharma.Web.Data.Entities;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        IQueryable GetByAppointment(int apptId);
    }
}
