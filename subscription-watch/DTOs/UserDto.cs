using subscription_watch.Enums;
using subscription_watch.Models;
using System.ComponentModel.DataAnnotations;

namespace subscription_watch.DTOs
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public int DefaultRemindDaysBefore { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public static UserDto FromEntity(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                DefaultRemindDaysBefore = user.DefaultRemindDaysBefore,
                CreatedAt = user.CreatedAtUtc
            };
        }
    }
}
