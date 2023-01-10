using Medpharma.Web.Data.Entities;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public interface IAdmissionRepository : IGenericRepository<Admission>
    {
        Task<Admission> GetByIdWithInfo(int id);
    }
}
