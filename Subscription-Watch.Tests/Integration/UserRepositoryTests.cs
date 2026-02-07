using Microsoft.Extensions.DependencyInjection;
using subscription_watch.Data;
using subscription_watch.Enums;
using subscription_watch.Models;
using subscription_watch.Repositories;
using Subscription_Watch.Tests.Fixtures;

namespace Subscription_Watch.Tests.Integration
{
    [Collection("PostgreSql")]
    public class UserRepositoryTests : IClassFixture<PostgreSqlContainerFixture>, IAsyncLifetime
    {
        private readonly UserFixture _userFixture;
        private readonly PostgreSqlContainerFixture _containerFixture;

        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _repository;

        // Testing data (copies from fixture)
        private List<User> _testUsers = null!;

        public UserRepositoryTests(PostgreSqlContainerFixture containerFixture)
        {
            _containerFixture = containerFixture;
            _userFixture = new UserFixture();

            _scope = _containerFixture.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _repository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        }

        public async Task InitializeAsync()
        {
            // Clear and seed database before each test
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();

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
        public async Task AddAsync_WhenUserValid_ShouldAddUserToDatabase()
        {
            var user = new User
            {
                Id = 1,
                Login = "UniqueFruit",
                FullName = "Gaylord Robinson",
                Email = "Gayrob@gmail.com",
                Password = "1HJSF8SHF26FJKSOF0F211VV-D0JFASSJ75AQ",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3,
                CreatedAtUtc = new DateTime(2025, 2, 12, 12, 45, 32).ToUniversalTime()
            };

            await _repository.AddUserAsync(user);

            var userInDb = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(userInDb);
            Assert.Equal(user.Login, userInDb.Login);
            Assert.Equal(user.Email, userInDb.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            var expectedUser = _testUsers[0];

            var userInDb = await _repository.GetUserByIdAsync(2);

            Assert.NotNull(userInDb);
            Assert.Equal(expectedUser.Id, userInDb.Id);
        }

        [Fact]
        public async Task GetByLoginAsync_WhenUserExists_ShouldReturnUser()
        {
            var expectedUser = _testUsers[0];

            var userInDb = await _repository.GetUserByLoginAsync("SeaLard");

            Assert.NotNull(userInDb);
            Assert.Equal(expectedUser.Login, userInDb.Login);
        }

        [Fact]
        public async Task ExistsAsync_WhenUserExists_ShouldReturnTrue()
        {
            var user = _testUsers[0];

            var exists = await _repository.UserExistsAsync(user.Login);

            Assert.True(exists);
        }

        [Fact]
        public async Task RemoveAsync_WhereUserValid_ShouldReturnNone()
        {
            var userBeforeDelete = await _repository.GetUserByIdAsync(2);
            Assert.NotNull(userBeforeDelete);

            await _repository.RemoveUserAsync(userBeforeDelete);

            var userAfterDelete = await _repository.GetUserByIdAsync(2);
            var existsByLogin = await _repository.UserExistsAsync(userBeforeDelete.Login);

            Assert.Null(userAfterDelete); 
            Assert.False(existsByLogin);
        }
    }
}
