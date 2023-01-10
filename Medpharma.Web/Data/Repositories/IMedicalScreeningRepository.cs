using Medpharma.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public interface IMedicalScreeningRepository : IGenericRepository<MedicalScreening>
    {
        List<MedicalScreening> GetAllWithInfo();
        Task<MedicalScreening> GetByIdWithInfo(int id);
        IQueryable<MedicalScreening> GetBySpeciality(int specialityId);
    }
}
