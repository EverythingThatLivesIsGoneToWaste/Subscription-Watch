using subscription_watch.Enums;
using System.ComponentModel.DataAnnotations;

namespace subscription_watch.Models
{
    // Service user (regular user or admin)
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        [Range(1, 15, ErrorMessage = "Напоминание: 1-15 дней")]
        public int DefaultRemindDaysBefore { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public List<UserSubscription> UserSubscriptions { get; set; } = [];
        public List<Reminder> Reminders { get; set; } = [];
    }
}
