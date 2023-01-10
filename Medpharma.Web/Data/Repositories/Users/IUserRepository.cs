using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories.Users
{
    public interface IUserRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistAsync(string id);
        IQueryable<T> GetAllAsQueryable();
        Task<T> GetUserByEmailAsync(string email);
    }
}