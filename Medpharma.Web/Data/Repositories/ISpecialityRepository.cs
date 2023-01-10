using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public interface ISpecialityRepository : IGenericRepository<Speciality>
    {
        IEnumerable<SelectListItem> GetComboSpeciality();
        IQueryable<Speciality> GetFilledSpecialities();
    }
}
