using Medpharma.Web.Data.Entities;

namespace Medpharma.Web.Data.Repositories
{
    public class TimeslotRepository : GenericRepository<Timeslot>, ITimeslotRepository
    {
        private readonly DataContext _context;
        public TimeslotRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
