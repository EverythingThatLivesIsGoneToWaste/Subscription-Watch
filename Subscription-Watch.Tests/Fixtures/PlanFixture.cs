using Microsoft.EntityFrameworkCore;
using subscription_watch.Data;
using subscription_watch.Enums;
using subscription_watch.Models;

namespace Subscription_Watch.Tests.Fixtures
{
    internal class PlanFixture
    {
        public List<Category> TestCategories { get; private set; }
        public List<SubscriptionPlan> TestSubscriptionPlans { get; private set; }

        public PlanFixture()
        {
            TestCategories =
            [
                new Category
                    {
                        Name = "Test Category",
                        Description = "Detailed description",
                        Color = 0xFFadd2aa
                    }
            ];

            TestSubscriptionPlans =
            [
                new SubscriptionPlan
                    {
                        Title = "Test subscription plan 1",
                        Description = "Very detailed description",
                        DefaultPrice = 8.0M,
                        DefaultCurrency = Currency.EUR,
                        BillingPeriod = BillingPeriod.Year,
                        IsActive = true,
                        Category = TestCategories[0]
                    },
                new SubscriptionPlan 
                    {
                        Title = "Test subscription plan 2",
                        Description = "Basic description",
                        DefaultPrice = 145.5M,
                        DefaultCurrency = Currency.RUB,
                        BillingPeriod = BillingPeriod.Month,
                        IsActive = true,
                        Category = TestCategories[0]
                    }
            ];
        }

        public List<Category> GetCopyOfTestCategories()
        {
            return [.. TestCategories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color
            })];
        }

        public List<SubscriptionPlan> GetCopyOfTestSubscriptionPlans()
        {
            return [.. TestSubscriptionPlans.Select(sp => new SubscriptionPlan
            {
                Id = sp.Id,
                Title = sp.Title,
                Description = sp.Description,
                DefaultPrice = sp.DefaultPrice,
                DefaultCurrency = sp.DefaultCurrency,
                BillingPeriod = sp.BillingPeriod,
                IsActive = sp.IsActive,
                Category = sp.Category
            })];
        }

        public async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Categories.AddRangeAsync(TestCategories);
            await context.SubscriptionPlans.AddRangeAsync(TestSubscriptionPlans);
            await context.SaveChangesAsync();
        }
    }
}
