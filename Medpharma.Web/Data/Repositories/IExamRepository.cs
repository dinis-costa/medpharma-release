using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Medpharma.Web.Data.Repositories
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        IEnumerable<SelectListItem> GetComboExams();
    }
}
