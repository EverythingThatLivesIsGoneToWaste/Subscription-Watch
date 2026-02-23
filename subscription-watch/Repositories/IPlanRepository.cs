using subscription_watch.Models;
using System.Numerics;

namespace subscription_watch.Repositories
{
    public interface IPlanRepository
    {
        Task<List<SubscriptionPlan>> GetSubscriptionPlansAsync(string? searchStr = null);
        Task<SubscriptionPlan?> GetPlanByTitleAsync(string title);
        Task<SubscriptionPlan?> GetPlanByIdAsync(int id);
        Task AddPlanAsync(SubscriptionPlan plan);
        Task<bool> PlanExistsAsync(string title);
        Task UpdatePlanAsync(SubscriptionPlan plan);
        Task RemovePlanAsync(SubscriptionPlan plan);
    }
}
