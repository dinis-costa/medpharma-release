using Medpharma.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories.Users
{
    public class UserRepository<T> : IUserRepository<T> where T : class, IUser
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<T> GetUserByEmailAsync(string email)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Email == email);
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }


        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }

        public async Task<bool> ExistAsync(string id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}