using subscription_watch.DTOs;
using subscription_watch.Models;
using subscription_watch.Repositories;

namespace subscription_watch.Services
{
    public class RegistrationService (
        IUserRepository userRepository,
        IPasswordHasher passwordHasher) : IRegistrationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<RegistrationResponse> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Login) || dto.Login.Length < 3)
                return RegistrationResponse.InvalidLogin();
            
            if (await _userRepository.UserExistsAsync(dto.Login))
                return RegistrationResponse.LoginTaken();

            if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 6)
                return RegistrationResponse.InvalidPassword();
            

            if (!string.IsNullOrEmpty(dto.Email))
            {
                var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
                if (emailExists)
                    return RegistrationResponse.EmailAlreadyExists();
            }

            var user = new User
            {
                Login = dto.Login,
                FullName = dto.FullName,
                Email = dto.Email,
                Password = _passwordHasher.HashPassword(dto.Password),
                Role = dto.Role,
                DefaultRemindDaysBefore = dto.DefaultRemindDaysBefore,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            return RegistrationResponse.Success(UserDto.FromEntity(user)
            );
        }
    }
}
