using subscription_watch.Data;
using subscription_watch.Enums;
using subscription_watch.Models;

namespace Subscription_Watch.Tests.Fixtures
{
    public class UserFixture
    {
        public List<User> TestUsers { get; private set; } =
        [
            // Initial test user
            new User()
            {
                Id = 2,
                Login = "SeaLard",
                FullName = "Light Yagami",
                Email = "IamJustice123@gmail.com",
                Password = "1HJSF8SHF26FJKSOF0F211VV-D0JFASSJ75AQ",
                Role = UserRole.User,
                DefaultRemindDaysBefore = 3,
                CreatedAtUtc = new DateTime(2025, 2, 12, 12, 45, 32).ToUniversalTime()
            }
        ];

        public List<User> GetCopyOfTestUsers()
        {
            return [.. TestUsers.Select(u => new User
            {
                Id = u.Id,
                Login = u.Login,
                FullName = u.FullName,
                Email = u.Email,
                Password = u.Password,
                Role = u.Role,
                DefaultRemindDaysBefore = u.DefaultRemindDaysBefore,
                CreatedAtUtc = u.CreatedAtUtc
            })];
        }

        public async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Users.AddRangeAsync(TestUsers);
            await context.SaveChangesAsync();
        }
    }
}
