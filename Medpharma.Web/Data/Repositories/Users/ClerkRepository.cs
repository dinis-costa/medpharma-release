using Medpharma.Web.Data.Entities;

namespace Medpharma.Web.Data.Repositories.Users
{
    public class ClerkRepository : UserRepository<Clerk>, IClerkRepository
    {
        private readonly DataContext _context;

        public ClerkRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public bool GetClerkType(User user)
        {
            Clerk clerk = _context.Clerks.Find(user.Id);

            var type = clerk.WareHouse;

            return type;
        }
    }
}
