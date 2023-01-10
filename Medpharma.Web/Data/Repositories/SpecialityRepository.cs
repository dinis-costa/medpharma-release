using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class SpecialityRepository : GenericRepository<Speciality>, ISpecialityRepository
    {
        private readonly DataContext _context;

        public SpecialityRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Speciality> GetFilledSpecialities()
        {
            return _context.Doctors.Select(x => x.Speciality).Distinct();
        }


        public IEnumerable<SelectListItem> GetComboSpeciality()
        {
            var availableSpecialities = _context.Doctors.Select(x => x.Speciality);

            var list = availableSpecialities.Distinct().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a speciality...)",
                Value = "0"
            });

            return list;
        }
    }
}
