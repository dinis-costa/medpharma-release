using Medpharma.Web.Data.Entities;

namespace Medpharma.Web.Data.Repositories.Users
{
    public class CustomerRepository : UserRepository<Customer>, ICustomerRepository
    {
        private readonly DataContext _context;

        public CustomerRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
