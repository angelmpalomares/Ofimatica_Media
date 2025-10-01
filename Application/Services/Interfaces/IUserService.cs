using Application.Services.Implementations;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> Login(LoginDto loginDto);
        Task RegisterUser(RegisterUserDto registerUserDto);
        Task UpdateUser(int userId, UpdateUserDto updateUserDto, HttpContext httpContext);
        Task<GetUserDto?> GetUserById(int userId);
        Task DeactivateUser(int userId);
        Task ActivateUser(int userId);
        Task<PagedCollectionResponse<GetUserDto>> FilterUsers(GetUserFiltersDto filters);
    }
}
