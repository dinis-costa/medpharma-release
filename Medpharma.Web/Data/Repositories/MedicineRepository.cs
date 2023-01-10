using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class MedicineRepository : GenericRepository<Medicine>, IMedicineRepository
    {
        private readonly DataContext _context;

        public MedicineRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboMedicines()
        {
            var list = _context.Medicines.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "(Select a medicine...)",
                Value = "0"
            });

            return list;
        }

    }
}
