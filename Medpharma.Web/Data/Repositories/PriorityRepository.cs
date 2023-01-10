using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class PriorityRepository : GenericRepository<Priority>, IPriorityRepository
    {
        private readonly DataContext _context;

        public PriorityRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboPriorities()
        {
            var list = _context.Priorities.Distinct().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a priority...)",
                Value = "0"
            });

            return list;
        }
    }
}
