using subscription_watch.DTOs;

namespace subscription_watch.Services
{
    public interface IPlanService
    {
        Task CreatePlanAsync(PlanCreateDto dto);
        Task DeactivatePlanAsync(int id);
        Task UpdatePlanAsync(int id, PlanUpdateDto dto);
        Task<List<PlanDto>> GetAllActivePlansAsync();
        Task<PlanDto> GetPlanByIdAsync(int id);
    }
}
