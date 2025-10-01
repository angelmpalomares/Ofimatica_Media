using Infrastructure.Dtos;
using Infrastructure.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(UserModel user);
        void Update(UserModel user);
        Task SaveChangesAsync();
        Task<UserModel?> GetByIdAsync(int UserId);
        Task<IEnumerable<UserModel>> GetAllAsync();
        Task<UserModel?> GetByUsername(string username);
        Task<(IEnumerable<UserModel>, int totalCount)> FilterUsers(GetUserFiltersDto filters);
    }
}
