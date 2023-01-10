using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        private readonly DataContext _context;

        public ExamRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboExams()
        {
            var list = _context.Medicines.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select)",
                Value = "0"
            });

            return list;
        }
    }
}
