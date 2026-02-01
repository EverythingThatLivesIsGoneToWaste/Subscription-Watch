using Microsoft.EntityFrameworkCore;
using subscription_watch.Models;

namespace subscription_watch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();
        public DbSet<Reminder> Reminders => Set<Reminder>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: configure data models in OnModelCreating
        }
    }
}
