using Application.Services.Implementations;
using Infrastructure.Dtos;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Unit
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();

            // Creamos IConfiguration "simple" para inyectar en el servicio (si el servicio lo requiere)
            var inMemoryConfig = new Dictionary<string, string>
            {
                // aunque tu servicio usa Environment var para la clave JWT, dejamos esta config por si la necesita
                ["Jwt:Key"] = "TestVeryLongJwtKey_01234567890123456789"
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            // Aseguramos que la variable que usa GenerateJwtToken existe (según tu código original)
            Environment.SetEnvironmentVariable("OFIMATICA_JWT_KEY", "TestVeryLongJwtKey_01234567890123456789");

            // Instantiate the real service (primary-constructor or classic constructor)
            _userService = new UserService(_userRepoMock.Object, _configuration);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var plain = "Password123!";
            var hashed = BCrypt.Net.BCrypt.HashPassword(plain);

            var user = new UserModel
            {
                UserId = 1,
                Name = "test",
                Surname = "test",
                Username = "testuser",
                Password = hashed,
                Email = "t@example.com",
                IsActive = true,
                LoginRetries = 0,
                Role = UserRole.User
            };

            _userRepoMock.Setup(r => r.GetByUsername("testuser")).ReturnsAsync(user);

            var dto = new LoginDto { Username = "testuser", Password = plain };

            // Act
            var token = await _userService.Login(dto);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains(".", token); // it's a JWT-ish token with dots
            _userRepoMock.Verify(r => r.GetByUsername("testuser"), Times.Once);
        }

        [Fact]
        public async Task Login_WrongPassword_IncrementsRetriesAndThrowsUnauthorized()
        {
            // Arrange
            var hashed = BCrypt.Net.BCrypt.HashPassword("someOther");
            var user = new UserModel { UserId = 2, Name = "test123", Email = "test@test.com", Surname="tester", Username = "tester", Password = hashed, IsActive = true, LoginRetries = 0 };

            _userRepoMock.Setup(r => r.GetByUsername("tester")).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Update(It.IsAny<UserModel>()));
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new LoginDto { Username = "tester", Password = "incorrect-pass" };

            // Act / Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.Login(dto));

            // Verify that we incremented retries and saved
            _userRepoMock.Verify(r => r.Update(It.Is<UserModel>(u => u.LoginRetries > 0)), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_Valid_CallsAddAndSave()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "Password1!345?",
                Email = "new@example.com",
                Name = "New",
                Surname = "User"
            };

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<UserModel>())).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            await _userService.RegisterUser(dto);

            // Assert
            _userRepoMock.Verify(r => r.AddAsync(It.Is<UserModel>(u => u.Username == "newuser" && u.Email == "new@example.com")), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ActivateUser_WhenExists_SetsIsActiveTrue()
        {
            // Arrange
            var user = new UserModel { UserId = 10, Name = "test123", Email = "test@test.com", Surname = "tester", Username = "tester", Password = "hashed123?", IsActive = false, LoginRetries = 0 };
            _userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Update(It.IsAny<UserModel>()));
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _userService.ActivateUser(10);

            // Assert
            Assert.True(user.IsActive);
            _userRepoMock.Verify(r => r.Update(user), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ActivateUser_NotFound_ThrowsKeyNotFound()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((UserModel?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ActivateUser(999));
        }
    }
}