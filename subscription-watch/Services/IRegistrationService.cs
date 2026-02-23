using subscription_watch.DTOs;

namespace subscription_watch.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDto dto);
    }
}
