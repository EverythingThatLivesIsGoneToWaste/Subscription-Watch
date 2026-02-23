using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using subscription_watch.Data;
using subscription_watch.Enums;
using subscription_watch.Models;
using subscription_watch.Repositories;
using Subscription_Watch.Tests.Fixtures;

namespace Subscription_Watch.Tests.Integration
{
    [Collection("PostgreSql")]
    public class PlanRepositoryTests : IAsyncLifetime
    {
        private readonly PlanFixture _planFixture;
        private readonly PostgreSqlContainerFixture _containerFixture;

        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPlanRepository _repository;

        // Testing data (copies from fixture)
        private List<SubscriptionPlan> _testSubscriptionPlans = null!;
        private List<Category> _testCategories = null!;

        public PlanRepositoryTests(PostgreSqlContainerFixture fixture)
        {
            _containerFixture = fixture;
            _planFixture = new PlanFixture();

            _scope = _containerFixture.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _repository = _scope.ServiceProvider.GetRequiredService<IPlanRepository>();
        }

        public async Task InitializeAsync()
        {
            // Clear and seed database before each test
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();

            await _planFixture.SeedAsync(_dbContext);

            _testSubscriptionPlans = _planFixture.GetCopyOfTestSubscriptionPlans();
            _testCategories = _planFixture.GetCopyOfTestCategories();
        }

        public Task DisposeAsync()
        {
            // Clear scope
            _scope?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddAsync_WhenPlanValid_ShouldAddPlanToDatabase()
        {
            var subscriptionPlan = new SubscriptionPlan {
                Title = "Test subscription plan 3",
                Description = "Very detailed description",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
                IsActive = true,
                CategoryId = _testCategories[0].Id
            };

            await _repository.AddPlanAsync(subscriptionPlan);

            var planInDb = await _repository.GetPlanByTitleAsync(subscriptionPlan.Title);
            Assert.NotNull(planInDb);
            Assert.Equal(subscriptionPlan.Title, planInDb.Title);
            Assert.Equal(subscriptionPlan.Description, planInDb.Description);
            Assert.True(await _repository.PlanExistsAsync(planInDb.Title));
        }

        [Fact]
        public async Task GetPlansAsync_WhenStringNotEmpty_ShouldReturnMatchingPlan()
        {
            var subscriptionPlans = await _repository.GetSubscriptionPlansAsync("detailed");

            Assert.NotNull(subscriptionPlans);
            Assert.Single(subscriptionPlans);
        }

        [Fact]
        public async Task GetPlansAsync_WhenStringEmpty_ShouldReturnAllPlans()
        {
            var subscriptionPlans = await _repository.GetSubscriptionPlansAsync();

            Assert.NotNull(subscriptionPlans);
            Assert.Equal(2, subscriptionPlans.Count);
        }

        [Fact]
        public async Task GetByTitleAsync_WhenPlanExists_ShouldReturnPlan()
        {
            var expectedPlan = _testSubscriptionPlans[0];

            var planInDb = await _repository.GetPlanByTitleAsync(expectedPlan.Title);

            Assert.NotNull(planInDb);
            Assert.Equal(expectedPlan.Title, planInDb.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WhenPlanExists_ShouldReturnPlan()
        {
            var expectedPlan = _testSubscriptionPlans[0];

            var planInDb = await _repository.GetPlanByIdAsync(expectedPlan.Id);

            Assert.NotNull(planInDb);
            Assert.Equal(expectedPlan.Id, planInDb.Id);
        }

        [Fact]
        public async Task ExistsAsync_WhenPlanExists_ShouldReturnTrue()
        {
            var plan = _testSubscriptionPlans[0];

            var exists = await _repository.PlanExistsAsync(plan.Title);

            Assert.True(exists);
        }

        [Fact]
        public async Task RemoveAsync_WherePlanValid_ShouldDeleteAndReturnNone()
        {
            var planId = _testSubscriptionPlans[0].Id;

            var planBeforeDelete = await _repository.GetPlanByIdAsync(planId);
            Assert.NotNull(planBeforeDelete);

            await _repository.RemovePlanAsync(planBeforeDelete);

            var planAfterDelete = await _repository.GetPlanByIdAsync(planId);
            var existsByLogin = await _repository.PlanExistsAsync(planBeforeDelete.Title);

            Assert.Null(planAfterDelete);
            Assert.False(existsByLogin);
        }
    }
}
