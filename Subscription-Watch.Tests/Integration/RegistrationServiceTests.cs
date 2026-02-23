using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using subscription_watch.Data;
using subscription_watch.DTOs;
using subscription_watch.Enums;
using subscription_watch.Models;
using subscription_watch.Repositories;
using subscription_watch.Services;
using Subscription_Watch.Tests.Fixtures;

namespace Subscription_Watch.Tests.Integration
{
    [Collection("PostgreSql")]
    public class RegistrationServiceTests : IAsyncLifetime
    {
        private readonly UserFixture _userFixture;
        private readonly PostgreSqlContainerFixture _containerFixture;

        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRegistrationService _service;
        private readonly IUserRepository _repository;

        // Testing data (copies from fixture)
        private List<User> _testUsers = null!;

        public RegistrationServiceTests(PostgreSqlContainerFixture containerFixture) {
            _containerFixture = containerFixture;
            _userFixture = new UserFixture();

            _scope = _containerFixture.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _service = _scope.ServiceProvider.GetRequiredService<IRegistrationService>();
            _repository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        }

        public async Task InitializeAsync()
        {
            // Clear and seed database before each test
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();

            await _userFixture.SeedAsync(_dbContext);

            _testUsers = _userFixture.GetCopyOfTestUsers();
        }

        public Task DisposeAsync()
        {
            // Clear scope
            _scope?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task RegisterAsync_MinimalValidCredentials_ShouldRegister()
        {
            var registerDto = new RegisterDto
            {
                Login = "Amigo",
                FullName = "Charlie",
                Password = "verystrongpswd1999",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3
            };

            var registrationResponse = await _service.RegisterAsync(registerDto);

            // Assert - response
            Assert.True(registrationResponse.IsSuccess);
            Assert.Equal(RegistrationStatus.Success, registrationResponse.Status);
            Assert.NotNull(registrationResponse.User);

            // Assert - UserDto
            Assert.Equal(registerDto.Login, registrationResponse.User.Login);
            Assert.Equal(registerDto.FullName, registrationResponse.User.FullName);
            Assert.Equal(registerDto.Role, registrationResponse.User.Role);

            // Assert - database
            var registeredUser = await _repository.GetUserByLoginAsync(registerDto.Login);
            Assert.NotNull(registeredUser);
            Assert.NotEqual(registerDto.Password, registeredUser.Password);
            Assert.Equal(registerDto.FullName, registeredUser.FullName);

            Assert.True(registeredUser.CreatedAtUtc <= DateTime.UtcNow);
        }

        [Fact]
        public async Task RegisterAsync_AllValidCredentials_ShouldRegister()
        {
            var registerDto = new RegisterDto
            {
                Login = "Amigo",
                FullName = "Charlie",
                Email = "charliework@gmail.com",
                Password = "verystrongpswd1999",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3
            };

            var registrationResponse = await _service.RegisterAsync(registerDto);

            // Assert - response
            Assert.True(registrationResponse.IsSuccess);
            Assert.Equal(RegistrationStatus.Success, registrationResponse.Status);
            Assert.NotNull(registrationResponse.User);

            // Assert - UserDto
            Assert.Equal(registerDto.Login, registrationResponse.User.Login);
            Assert.Equal(registerDto.FullName, registrationResponse.User.FullName);
            Assert.Equal(registerDto.Role, registrationResponse.User.Role);
            Assert.Equal(registerDto.Email, registrationResponse.User.Email);

            // Assert - database
            var registeredUser = await _repository.GetUserByLoginAsync(registerDto.Login);
            Assert.NotNull(registeredUser);
            Assert.NotEqual(registerDto.Password, registeredUser.Password);
            Assert.Equal(registerDto.FullName, registeredUser.FullName);
            Assert.Equal(registerDto.Email, registeredUser.Email);

            Assert.True(registeredUser.CreatedAtUtc <= DateTime.UtcNow);
        }

        [Fact]
        public async Task RegisterAsync_ExistingLogin_ShouldReturnLoginTakenResponse()
        {
            var registerDto = new RegisterDto
            {
                Login = _testUsers[0].Login,
                FullName = "Charlie",
                Email = "charliework@gmail.com",
                Password = "verystrongpswd1999",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3
            };

            var registrationResponse = await _service.RegisterAsync(registerDto);

            // Assert - response
            Assert.False(registrationResponse.IsSuccess);
            Assert.Equal(RegistrationStatus.LoginAlreadyExists, registrationResponse.Status);
            Assert.Null(registrationResponse.User);

            // Assert - database
            var existingUser = await _repository.GetUserByLoginAsync(registerDto.Login);
            Assert.NotNull(existingUser);
            Assert.Equal(_testUsers[0].Login, existingUser.Login);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ShouldReturnEmailAlreadyExistsResponse()
        {
            var registerDto = new RegisterDto
            {
                Login = "Amigo",
                FullName = "Charlie",
                Email = _testUsers[0].Email!,
                Password = "verystrongpswd1999",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3
            };

            var registrationResponse = await _service.RegisterAsync(registerDto);

            // Assert - response
            Assert.False(registrationResponse.IsSuccess);
            Assert.Equal(RegistrationStatus.EmailAlreadyExists, registrationResponse.Status);
            Assert.Null(registrationResponse.User);

            // Assert - database
            var existingUser = await _repository.GetUserByLoginAsync(registerDto.Login);
            Assert.Null(existingUser);
        }
    }
}
