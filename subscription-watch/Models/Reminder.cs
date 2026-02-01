using subscription_watch.Enums;

namespace subscription_watch.Models
{
    // Payment reminder
    public class Reminder
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public ReminderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ShownAt { get; set; }
        public NotificationType NotificationType { get; set; }

        public int UserSubscriptionId { get; set; }
        public int UserId { get; set; }

        public UserSubscription UserSubscription { get; set; } = new();
        public User User { get; set; } = new();
    }
}
