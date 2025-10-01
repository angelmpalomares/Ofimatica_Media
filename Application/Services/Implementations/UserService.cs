using Application.Services.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ValidationResult = Infrastructure.Validation.ValidationResult;

namespace Application.Services.Implementations
{
    public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;

        #region Login
        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsername(loginDto.Username) ?? throw new KeyNotFoundException("ErrorMessage.UserDoesntExist");
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("ErrorMessage.InactiveUser");
            }
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                user.LoginRetries++;
                if(user.LoginRetries == 3) 
                {
                    user.IsActive = false;
                }
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                throw new UnauthorizedAccessException("ErrorMessage.InvalidCredentials");
            }
            if (user.LoginRetries != 0)
            {
                user.LoginRetries = 0;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }
            return GenerateJwtToken(user);
        }
        private string GenerateJwtToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims : claims,
                expires : DateTime.Now.AddMinutes(45),
                signingCredentials :credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        public async Task RegisterUser(RegisterUserDto registerUserDto)
        {
            try
            {
                await ValidateUserData(registerUserDto);
            }
            catch (ValidationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            var newUser = new UserModel
            {
                Email = registerUserDto.Email,
                Name = registerUserDto.Name,
                Surname = registerUserDto.Surname,
                Username = registerUserDto.Username,
                Role = UserRole.User,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            };
            try
            {
                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if(ex.InnerException is MySqlException mySqlException)
                {
                    if (mySqlException.Message.Contains("Duplicate entry") && mySqlException.Message.Contains("IX_Users_Email"))
                    {
                        throw new ValidationException("ErrorMessage.Email_Duplicated");
                    }
                    if (mySqlException.Message.Contains("Duplicate entry") && mySqlException.Message.Contains("IX_Users_Username"))
                    {
                        throw new ValidationException("ErrorMessage.Username_Duplicated");
                    }
                }
            }
        }

        public async Task UpdateUser(int userId, UpdateUserDto updateUserDto, HttpContext httpContext)
        {
            int currentUserId = int.Parse(httpContext.User.FindFirst("UserId")?.Value ?? "0");
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (userId != currentUserId && currentUser.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("ErrorMessage.UnauthorizedUpdate");
            }
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("ErrorMessage.UserDoesntExist");
            if (UserValidator.ValidateEmail(updateUserDto.Email,true) != ValidationResult.Success() ||
                UserValidator.ValidatePassword(updateUserDto.Password, true) != ValidationResult.Success())
            {
                throw new ValidationException("ErrorMessage.EmailOrPasswordIncorrect");
            }
            if (updateUserDto.Email != null) user.Email = updateUserDto.Email;
            if (updateUserDto.Name != null) user.Name = updateUserDto.Name;
            if (updateUserDto.Surname != null) user.Surname = updateUserDto.Surname;
            if (updateUserDto.Password != null) user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            if (updateUserDto.Username != null) user.Username = updateUserDto.Username;
            try
            {
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is MySqlException mySqlException)
                {
                    if (mySqlException.Message.Contains("Duplicate entry") && mySqlException.Message.Contains("IX_Users_Email"))
                    {
                        throw new ValidationException("ErrorMessage.Email_Duplicated");
                    }
                    if (mySqlException.Message.Contains("Duplicate entry") && mySqlException.Message.Contains("IX_Users_Username"))
                    {
                        throw new ValidationException("ErrorMessage.Username_Duplicated");
                    }
                }
            }
        }
        public async Task<GetUserDto?> GetUserById(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return MapToDto(user);
        }
        public async Task DeactivateUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("ErrorMessage.UserDoesntExist");
            user.IsActive = false;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
        public async Task ActivateUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("ErrorMessage.UserDoesntExist");
            user.IsActive = true;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
        public async Task<PagedCollectionResponse<GetUserDto>> FilterUsers(GetUserFiltersDto filters)
        {
            var (users, totalCount) = await _userRepository.FilterUsers(filters);
            var userDtos = users.Select(_ => MapToDto(_)).ToList() ?? [];
            return new PagedCollectionResponse<GetUserDto>(){ Items = userDtos, Total = totalCount};
        }
        #region Helpers
        private static Task ValidateUserData(RegisterUserDto userDto, bool isUpdate = false)
        {
            var errors = new List<string>();
            var emailValidation = UserValidator.ValidateEmail(userDto.Email, isUpdate);
            var passwordValidation = UserValidator.ValidatePassword(userDto.Password, isUpdate);
            if (!emailValidation.IsValid) errors.AddRange(emailValidation.Errors);
            if (!passwordValidation.IsValid) errors.AddRange(passwordValidation.Errors);
            if (errors.Count != 0)
            {
                throw new ValidationException(string.Join(", ", errors));
            }
            return Task.CompletedTask;
        }
        private static GetUserDto MapToDto(UserModel model) 
        {
            return new GetUserDto
            {
                UserId = model.UserId,
                Username = model.Username,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                IsActive = model.IsActive
            };
        }
        #endregion
    }
}
