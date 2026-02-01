using subscription_watch.Enums;

namespace subscription_watch.Models
{
    // Service user (regular user or admin)
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public int DefaultRemindDaysBefore { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
