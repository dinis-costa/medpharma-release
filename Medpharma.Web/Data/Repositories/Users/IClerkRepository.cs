using Medpharma.Web.Data.Entities;

namespace Medpharma.Web.Data.Repositories.Users
{
    public interface IClerkRepository : IUserRepository<Clerk>
    {
        bool GetClerkType(User user);
    }
}
