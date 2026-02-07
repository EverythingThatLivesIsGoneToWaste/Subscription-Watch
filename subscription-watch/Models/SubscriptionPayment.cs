using subscription_watch.Enums;

namespace subscription_watch.Models
{
    // Payment history record
    public class SubscriptionPayment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public PaymentStatus Status { get; set; }
        public int UserSubscriptionId { get; set; }

        public UserSubscription UserSubscription { get; set; } = new();
    }
}
