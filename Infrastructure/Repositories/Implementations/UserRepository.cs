using Infrastructure.Database;
using Infrastructure.Models;
using Infrastructure.Dtos;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository(InfraDbContext context) : BaseRepository<UserModel>(context), IUserRepository
    {
        public async Task<UserModel?> GetByUsername(string username)
        {
            return await _context.Users
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
        }
        public async Task<(IEnumerable<UserModel>, int totalCount)> FilterUsers(GetUserFiltersDto filters)
        {
            int pageNumber = filters.PageNumber - 1;
            IQueryable<UserModel?> query = _context.Users;
            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var searchTerm = filters.Name.Trim().ToLower();
                query = query.Where(a => 
                    a.Name.ToLower().Contains(searchTerm) ||
                    a.Surname.ToLower().Contains(searchTerm) ||
                    (a.Name + " " + a.Surname).ToLower().Contains(searchTerm)
                );
            }
            if (!string.IsNullOrWhiteSpace(filters.Username))
            {
                query = query.Where(a => a.Username.ToLower().Contains(filters.Username.Trim().ToLower()));
            }
            if (filters.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == filters.IsActive.Value);
            }
            var total = await query.CountAsync();
            var users = await query
                .Skip(filters.PageSize * pageNumber)
                .Take(filters.PageSize)
                .ToListAsync();
            return (users, total);
        }
    }
}
