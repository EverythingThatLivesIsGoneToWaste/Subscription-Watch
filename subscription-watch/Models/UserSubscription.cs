using subscription_watch.Enums;

namespace subscription_watch.Models
{
    // Actual user subscription
    public class UserSubscription
    {
        public int Id { get; set; }
        public decimal ActualPrice { get; set; }
        public Currency ActualCurrency { get; set; }
        public DateTime NextBillingDate { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public int RemindDaysBefore { get; set; }
        public bool AutoRenew { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }

        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }

        public User User { get; set; } = new();
        public SubscriptionPlan SubscriptionPlan { get; set; } = new();

        public List<SubscriptionPayment> SubscriptionPayments { get; set; } = [];
        public List<Reminder> Reminders { get; set; } = [];
    }
}
