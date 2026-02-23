using Microsoft.EntityFrameworkCore;
using subscription_watch.Data;
using subscription_watch.Models;

namespace subscription_watch.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPlan>> GetSubscriptionPlansAsync(string? searchStr = null)
        {
            var query = _context.SubscriptionPlans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                var lowerSearch = searchStr.ToLower();

                query = query.Where(sp =>
                    sp.Title.ToLower().Contains(lowerSearch) ||
                    sp.Description.ToLower().Contains(lowerSearch));
            }

            return await query.ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetPlanByTitleAsync(string title)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Title.ToLower() == title.ToLower());
        }

        public async Task<SubscriptionPlan?> GetPlanByIdAsync(int id)
        {
            return await _context.SubscriptionPlans.FindAsync(id);
        }

        public async Task AddPlanAsync(SubscriptionPlan plan)
        {
            await _context.SubscriptionPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PlanExistsAsync(string title)
        {
            return await _context.SubscriptionPlans
                .AnyAsync(u => u.Title.ToLower() == title.ToLower());
        }

        public async Task UpdatePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }
}
