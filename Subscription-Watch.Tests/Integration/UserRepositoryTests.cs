using Microsoft.Extensions.DependencyInjection;
using subscription_watch.Repositories;
using Subscription_Watch.Tests.Fixtures;
using subscription_watch.Data;
using subscription_watch.Models;
using subscription_watch.Enums;

namespace Subscription_Watch.Tests.Integration
{
    [Collection("PostgreSql")]
    public class UserRepositoryTests : IClassFixture<PostgreSqlContainerFixture>, IAsyncLifetime
    {
        private readonly PostgreSqlContainerFixture _fixture;
        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _dbContext;

        public UserRepositoryTests(PostgreSqlContainerFixture fixture)
        {
            _fixture = fixture;

            _scope = _fixture.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task InitializeAsync()
        {
            // Clear database before each test (optional)
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();
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
            using var scope = _fixture.ServiceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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

            await repository.AddUserAsync(user);

            var userInDb = await dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(userInDb);
            Assert.Equal(user.Login, userInDb.Login);
            Assert.Equal(user.Email, userInDb.Email);
        }

        // TODO: Implement all user-related tests
    }
}
