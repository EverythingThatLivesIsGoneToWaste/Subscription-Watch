using subscription_watch.Models;

namespace subscription_watch.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string login, string password);
        Task LoginAsync(User user);
        Task LogoutAsync();
    }
}
