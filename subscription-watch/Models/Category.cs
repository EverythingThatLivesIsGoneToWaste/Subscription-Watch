namespace subscription_watch.Models
{
    // Subscription category
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Color { get; set; }
        public List<SubscriptionPlan> SubscriptionPlans { get; set; } = [];
    }
}
