using subscription_watch.Enums;

namespace subscription_watch.DTOs
{
    public class RegistrationResponse
    {
        public bool IsSuccess { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? Message { get; set; }
        public UserDto? User { get; set; }
        public string[]? Errors { get; set; }

        public static RegistrationResponse Success(UserDto userDto)
        {
            return new RegistrationResponse
            {
                IsSuccess = true,
                Status = RegistrationStatus.Success,
                Message = "Registration completed successfully",
                User = userDto
            };
        }

        public static RegistrationResponse LoginTaken()
        {
            return new RegistrationResponse
            {
                IsSuccess = false,
                Status = RegistrationStatus.LoginAlreadyExists,
                Message = "This login is already taken",
                Errors = ["login_already_exists"]
            };
        }

        public static RegistrationResponse InvalidLogin()
        {
            return new RegistrationResponse
            {
                IsSuccess = false,
                Status = RegistrationStatus.InvalidLogin,
                Message = "Login must be at least 3 characters long",
                Errors = ["invalid_login_format"]
            };
        }

        public static RegistrationResponse InvalidPassword()
        {
            return new RegistrationResponse
            {
                IsSuccess = false,
                Status = RegistrationStatus.InvalidPassword,
                Message = "Password must be at least 6 characters long",
                Errors = ["invalid_password_format"]
            };
        }

        public static RegistrationResponse EmailAlreadyExists()
        {
            return new RegistrationResponse
            {
                IsSuccess = false,
                Status = RegistrationStatus.EmailAlreadyExists,
                Message = "This email is already taken",
                Errors = ["email_already_exists"]
            };
        }
    }
}