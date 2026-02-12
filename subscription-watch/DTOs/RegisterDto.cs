using subscription_watch.Enums;
using System.ComponentModel.DataAnnotations;

namespace subscription_watch.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public int DefaultRemindDaysBefore { get; set; }
    }
}
