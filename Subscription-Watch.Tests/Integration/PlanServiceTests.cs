using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using subscription_watch.Data;
using subscription_watch.DTOs;
using subscription_watch.Enums;
using subscription_watch.Exceptions;
using subscription_watch.Models;
using subscription_watch.Repositories;
using subscription_watch.Services;
using Subscription_Watch.Tests.Fixtures;

namespace Subscription_Watch.Tests.Integration
{
    [Collection("PostgreSql")]
    public class PlanServiceTests : IAsyncLifetime
    {
        private readonly PlanFixture _planFixture;
        private readonly PostgreSqlContainerFixture _containerFixture;

        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPlanService _service;
        private readonly IPlanRepository _repository;

        // Testing data (copies from fixture)
        private List<SubscriptionPlan> _testSubscriptionPlans = null!;
        private List<Category> _testCategories = null!;

        public PlanServiceTests(PostgreSqlContainerFixture containerFixture) { 
            _planFixture = new PlanFixture();
            _containerFixture = containerFixture;

            _scope = _containerFixture.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _service = _scope.ServiceProvider.GetRequiredService<IPlanService>();
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
            _scope.Dispose();
            return Task.CompletedTask;
        }

        // Tests for CreatePlanAsync
        [Fact]
        public async Task CreateAsync_WhenPlanValid_ShouldAddPlanToDatabase()
        {
            var planDto = new PlanCreateDto
            {
                Title = "Test subscription plan 3",
                Description = "Very detailed description",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
                IsActive = true,
                CategoryId = _testCategories[0].Id
            };

            await _service.CreatePlanAsync(planDto);

            var planInDb = await _repository.GetPlanByTitleAsync(planDto.Title);
            Assert.NotNull(planInDb);
            Assert.Equal(planDto.Title, planInDb.Title);
            Assert.Equal(planDto.Description, planInDb.Description);
            Assert.Equal(planDto.DefaultPrice, planInDb.DefaultPrice);

            Assert.True(await _repository.PlanExistsAsync(planInDb.Title));
        }

        [Fact]
        public async Task CreateAsync_WhenPlanInvalid_ShouldThrowArgumentException()
        {
            var planDto = new PlanCreateDto
            {
                Title = "Te",
                Description = "Very detailed description",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
                IsActive = true,
                CategoryId = _testCategories[0].Id
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreatePlanAsync(planDto)
            );

            Assert.Equal($"Invalid title '{planDto.Title}'", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_WhenPlanExists_ShouldThrowAlreadyExistsException()
        {
            var planDto = new PlanCreateDto
            {
                Title = _testSubscriptionPlans[0].Title,
                Description = "Very detailed description",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
                IsActive = true,
                CategoryId = _testCategories[0].Id
            };

            var exception = await Assert.ThrowsAsync<PlanAlreadyExistsException>(
                () => _service.CreatePlanAsync(planDto)
            );

            Assert.Equal($"Plan with title '{planDto.Title}' already exists", exception.Message);

            var planInDb = await _repository.GetPlanByTitleAsync(planDto.Title);
            Assert.NotNull(planInDb);
            Assert.True(await _repository.PlanExistsAsync(planDto.Title));
        }

        // Tests for DeactivatePlanAsync
        [Fact]
        public async Task DeactivateAsync_WhenPlanActive_ShouldDeactivatePlan()
        {
            var plan = await _repository.GetPlanByTitleAsync(_testSubscriptionPlans[0].Title);
            Assert.NotNull(plan);
            Assert.True(plan.IsActive);

            await _service.DeactivatePlanAsync(plan.Id);

            Assert.False(plan.IsActive);
        }

        [Fact]
        public async Task DeactivateAsync_WhenPlanDoesNotExist_ShouldThrowNotFoundException()
        {
            var fakeId = 5000;

            var exception = await Assert.ThrowsAsync<PlanNotFoundException>(
                () => _service.DeactivatePlanAsync(fakeId)
            );

            Assert.Equal($"Plan with id '{fakeId}' not found", exception.Message);
        }

        [Fact]
        public async Task DeactivateAsync_WhenPlanInactive_ShouldThrowAlreadyDeactivatedException()
        {
            var plan = await _repository.GetPlanByTitleAsync(_testSubscriptionPlans[0].Title);
            Assert.NotNull(plan);
            Assert.True(plan.IsActive);

            plan.IsActive = false;
            await _repository.UpdatePlanAsync(plan);
            Assert.False(plan.IsActive);

            var exception = await Assert.ThrowsAsync<PlanAlreadyDeactivatedException>(
                () => _service.DeactivatePlanAsync(plan.Id)
            );

            Assert.Equal($"Plan with title '{plan.Title}' already deactivated", exception.Message);
        }

        // Tests for ActivatePlanAsync
        [Fact]
        public async Task ActivateAsync_WhenPlanInactive_ShouldActivatePlan()
        {
            var plan = await _repository.GetPlanByTitleAsync(_testSubscriptionPlans[0].Title);
            Assert.NotNull(plan);
            Assert.True(plan.IsActive);

            plan.IsActive = false;
            await _repository.UpdatePlanAsync(plan);
            Assert.False(plan.IsActive);

            await _service.ActivatePlanAsync(plan.Id);

            Assert.True(plan.IsActive);
        }

        [Fact]
        public async Task ActivateAsync_WhenPlanDoesNotExist_ShouldThrowNotFoundException()
        {
            var fakeId = 5000;

            var exception = await Assert.ThrowsAsync<PlanNotFoundException>(
                () => _service.ActivatePlanAsync(fakeId)
            );

            Assert.Equal($"Plan with id '{fakeId}' not found", exception.Message);
        }

        [Fact]
        public async Task ActivateAsync_WhenPlanActive_ShouldThrowAlreadyActiveException()
        {
            var plan = await _repository.GetPlanByTitleAsync(_testSubscriptionPlans[0].Title);
            Assert.NotNull(plan);
            Assert.True(plan.IsActive);

            var exception = await Assert.ThrowsAsync<PlanAlreadyActiveException>(
                () => _service.ActivatePlanAsync(plan.Id)
            );

            Assert.Equal($"Plan with title '{plan.Title}' already active", exception.Message);
            Assert.True(plan.IsActive);
        }

        // Tests for UpdatePlanAsync
        [Fact]
        public async Task UpdateAsync_WhenPlanValid_ShouldUpdatePlanInDatabase()
        {
            var planBeforeUpdate = _testSubscriptionPlans[0];

            var planUpdateDto = new PlanUpdateDto {
                Title = "Test subscription plan updated",
                Description = "None",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
            };

            Assert.NotEqual(planBeforeUpdate.Title, planUpdateDto.Title);
            Assert.NotEqual(planBeforeUpdate.Description, planUpdateDto.Description);
            Assert.NotEqual(planBeforeUpdate.DefaultPrice, planUpdateDto.DefaultPrice);

            await _service.UpdatePlanAsync(planBeforeUpdate.Id, planUpdateDto);
            if (planBeforeUpdate.Title != planUpdateDto.Title)
            {
                Assert.False(await _repository.PlanExistsAsync(planBeforeUpdate.Title));
            }

            var planAfterUpdate = await _repository.GetPlanByTitleAsync(planUpdateDto.Title);

            Assert.NotNull(planAfterUpdate);

            Assert.Equal(planUpdateDto.Title, planAfterUpdate.Title);
            Assert.Equal(planUpdateDto.Description, planAfterUpdate.Description);
            Assert.Equal(planUpdateDto.DefaultPrice, planAfterUpdate.DefaultPrice);
        }

        [Fact]
        public async Task UpdateAsync_WhenPlanDoesNotExist_ShouldThrowNotFoundException()
        {
            var fakeId = 5000;

            var planUpdateDto = new PlanUpdateDto
            {
                Title = "Test subscription plan updated",
                Description = "None",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
            };

            var exception = await Assert.ThrowsAsync<PlanNotFoundException>(
                () => _service.UpdatePlanAsync(fakeId, planUpdateDto)
            );

            Assert.Equal($"Plan with id '{fakeId}' not found", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenPlanWithNewTitleExists_ShouldThrowAlreadyExistsException()
        {
            var planBeforeUpdate = _testSubscriptionPlans[0];
            var takenTitle = _testSubscriptionPlans[1].Title;

            var planUpdateDto = new PlanUpdateDto
            {
                Title = takenTitle,
                Description = "None",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
            };

            Assert.NotEqual(planBeforeUpdate.Title, planUpdateDto.Title);
            Assert.NotEqual(planBeforeUpdate.Description, planUpdateDto.Description);
            Assert.NotEqual(planBeforeUpdate.DefaultPrice, planUpdateDto.DefaultPrice);

            var exception = await Assert.ThrowsAsync<PlanAlreadyExistsException>(
                () => _service.UpdatePlanAsync(planBeforeUpdate.Id, planUpdateDto)
            );

            Assert.Equal($"Plan with title '{planUpdateDto.Title}' already exists", exception.Message);
        }

        // Tests for GetAllActivePlansAsync
        [Fact]
        public async Task GetAllActiveAsync_WhenPlansExist_ShouldReturnAllActivePlans()
        {
            var plans = await _service.GetAllActivePlansAsync();
            Assert.Equal(2, plans.Count);

            var planDto = new PlanCreateDto
            {
                Title = "Test subscription plan 3",
                Description = "Very detailed description",
                DefaultPrice = 16.0M,
                DefaultCurrency = Currency.USD,
                BillingPeriod = BillingPeriod.Month,
                IsActive = false,
                CategoryId = _testCategories[0].Id
            };

            await _service.CreatePlanAsync(planDto);

            Assert.True(await _repository.PlanExistsAsync(planDto.Title));

            var plansExtended = await _service.GetAllActivePlansAsync();
            Assert.Equal(2, plansExtended.Count);

            var expectedTitles = _testSubscriptionPlans.Select(p => p.Title).OrderBy(t => t);
            var actualTitles = plans.Select(p => p.Title).OrderBy(t => t);
            Assert.Equal(expectedTitles, actualTitles);
        }

        // Tests for GetPlanByIdAsync
        [Fact]
        public async Task GetByIdAsync_WhenPlanExists_ShouldReturnPlan()
        {
            var planToRetrieve = _testSubscriptionPlans[0];

            var plan = await _service.GetPlanByIdAsync(planToRetrieve.Id);
            Assert.NotNull(plan);
            Assert.Equal(planToRetrieve.Title, plan.Title);
            Assert.Equal(planToRetrieve.Description, plan.Description);
            Assert.Equal(planToRetrieve.DefaultPrice, plan.DefaultPrice);
        }

        [Fact]
        public async Task GetByIdAsync_WhenPlanDoesNotExist_ShouldThrowNotFoundException()
        {
            var fakeId = 5000;

            var exception = await Assert.ThrowsAsync<PlanNotFoundException>(
                () => _service.GetPlanByIdAsync(fakeId)
            );

            Assert.Equal($"Plan with id '{fakeId}' not found", exception.Message);
        }
    }
}
