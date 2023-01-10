using Medpharma.Web.Data.Entities;
using System.Linq;

namespace Medpharma.Web.Data.Repositories.Users
{
    public interface IDoctorRepository : IUserRepository<Doctor>
    {
        Doctor GetById(string id);
        IQueryable<Doctor> GetTest(string id);
    }
}
