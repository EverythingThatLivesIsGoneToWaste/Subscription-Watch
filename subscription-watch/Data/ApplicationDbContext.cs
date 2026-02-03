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
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Login).IsUnique();

                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Password).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasConversion<string>();

                entity.Property(e => e.DefaultRemindDaysBefore).IsRequired().HasDefaultValue(3);

                entity.Property(e => e.CreatedAtUtc).IsRequired();
            });

            modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint
            ("CK_User_DefaultRemindDaysBefore", "DefaultRemindDaysBefore BETWEEN 1 AND 15"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserSubscriptions)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Reminders)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Color).IsRequired().HasDefaultValue(0xFFadd2aa);
            });

            modelBuilder.Entity<Category>()
                .HasMany(e => e.SubscriptionPlans)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // SubscriptionPlan entity configuration
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Title).IsUnique();

                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);

                entity.Property(e => e.DefaultPrice).IsRequired();

                entity.Property(e => e.DefaultCurrency).IsRequired().HasMaxLength(20).HasConversion<string>();

                entity.Property(e => e.BillingPeriod).IsRequired().HasMaxLength(20).HasConversion<string>();

                entity.Property(e => e.IsActive).IsRequired();
            });

            modelBuilder.Entity<SubscriptionPlan>()
                .HasMany(e=>e.UserSubscriptions)
                .WithOne(e=>e.SubscriptionPlan)
                .HasForeignKey(e=>e.SubscriptionPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // UserSubscription entity configuration
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ActualPrice).IsRequired();

                entity.Property(e => e.ActualCurrency).IsRequired().HasMaxLength(20).HasConversion<string>();

                entity.Property(e => e.NextBillingDate).IsRequired();

                entity.Property(e => e.LastBillingDate).IsRequired(false);

                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasConversion<string>();

                entity.Property(e => e.RemindDaysBefore).IsRequired().HasDefaultValue(3);

                entity.Property(e => e.AutoRenew).IsRequired();

                entity.Property(e => e.Notes).IsRequired().HasMaxLength(200);

                entity.Property(e => e.CreatedAtUtc).IsRequired();
            });

            modelBuilder.Entity<UserSubscription>()
                .HasMany(e => e.SubscriptionPayments)
                .WithOne(e => e.UserSubscription)
                .HasForeignKey(e => e.UserSubscriptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSubscription>()
                .HasMany(e => e.Reminders)
                .WithOne(e => e.UserSubscription)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // TODO: configure remaining entities
        }
    }
}
