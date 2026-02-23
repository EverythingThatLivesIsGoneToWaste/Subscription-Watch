using subscription_watch.Models;

namespace subscription_watch.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByLoginAsync(string login);
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string login);
        Task RemoveUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}
