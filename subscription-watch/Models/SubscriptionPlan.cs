using subscription_watch.Enums;

namespace subscription_watch.Models
{
    // Subscription plan template
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public Currency DefaultCurrency { get; set; }
        public BillingPeriod BillingPeriod { get; set; }
        public bool IsActive { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = new();

        public List<UserSubscription> UserSubscriptions { get; set; } = [];
    }
}
