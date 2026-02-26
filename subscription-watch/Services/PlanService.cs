using subscription_watch.DTOs;
using subscription_watch.Exceptions;
using subscription_watch.Models;
using subscription_watch.Repositories;

namespace subscription_watch.Services
{
    public class PlanService (IPlanRepository planRepository) : IPlanService
    {
        private readonly IPlanRepository _planRepository = planRepository;

        public async Task CreatePlanAsync(PlanCreateDto dto)
        {
            var title = dto.Title;

            if (string.IsNullOrEmpty(title) || title.Length < 3)
                throw new ArgumentException($"Invalid title '{title}'");

            if (await _planRepository.PlanExistsAsync(title))
                throw new PlanAlreadyExistsException(title);

            var plan = new SubscriptionPlan { 
                Title = dto.Title,
                Description = dto.Description,
                DefaultPrice = dto.DefaultPrice,
                DefaultCurrency = dto.DefaultCurrency,
                BillingPeriod = dto.BillingPeriod,
                IsActive = dto.IsActive,
                CategoryId = dto.CategoryId,
            };

            await _planRepository.AddPlanAsync(plan);
        }

        public async Task DeactivatePlanAsync(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id)
                ?? throw new PlanNotFoundException(id);

            if (!plan.IsActive)
                throw new PlanAlreadyDeactivatedException(plan.Title);

            plan.IsActive = false;
            await _planRepository.UpdatePlanAsync(plan);
        }

        public async Task ActivatePlanAsync(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id)
                ?? throw new PlanNotFoundException(id);

            if (plan.IsActive)
                throw new PlanAlreadyActiveException(plan.Title);

            plan.IsActive = true;
            await _planRepository.UpdatePlanAsync(plan);
        }

        public async Task UpdatePlanAsync(int id, PlanUpdateDto dto)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id)
                ?? throw new PlanNotFoundException(id);

            if (plan.Title != dto.Title && await _planRepository.PlanExistsAsync(dto.Title))
                throw new PlanAlreadyExistsException(dto.Title);

            plan.Title = dto.Title;
            plan.Description = dto.Description;
            plan.DefaultPrice = dto.DefaultPrice;
            plan.DefaultCurrency = dto.DefaultCurrency;
            plan.BillingPeriod = dto.BillingPeriod;

            await _planRepository.UpdatePlanAsync(plan);
        }

        public async Task<List<PlanDto>> GetAllActivePlansAsync()
        {
            var plans = await _planRepository.GetSubscriptionPlansAsync();

            return [.. plans.Where(sp => sp.IsActive)
                .Select(sp => new PlanDto
                {
                    Title = sp.Title,
                    Description = sp.Description,
                    DefaultPrice = sp.DefaultPrice,
                    DefaultCurrency = sp.DefaultCurrency,
                    BillingPeriod = sp.BillingPeriod,
                    IsActive = sp.IsActive,
                    CategoryId = sp.CategoryId
                })];
        }

        public async Task<PlanDto> GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id)
                ?? throw new PlanNotFoundException(id);

            return new PlanDto
            {
                Title = plan.Title,
                Description = plan.Description,
                DefaultPrice = plan.DefaultPrice,
                DefaultCurrency = plan.DefaultCurrency,
                BillingPeriod = plan.BillingPeriod,
                IsActive = plan.IsActive,
                CategoryId = plan.CategoryId
            };
        }
    }
}
