using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Medpharma.Web.Data.Repositories
{
    public interface IPriorityRepository : IGenericRepository<Priority>
    {
        IEnumerable<SelectListItem> GetComboPriorities();
    }
}
