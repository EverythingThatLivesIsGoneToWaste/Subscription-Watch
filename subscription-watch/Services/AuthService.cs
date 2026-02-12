using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using subscription_watch.Models;
using subscription_watch.Repositories;
using subscription_watch.Exceptions;
using System.Security.Claims;

namespace subscription_watch.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<User?> AuthenticateAsync(string login, string password)
        {
            var user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null || !_passwordHasher.VerifyPassword(user.Password, password))
                throw new UnauthorizedException("Invalid login or password");

            return user;
        }

        public async Task LoginAsync(User user)
        {
            var claims = new List<Claim> {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.FullName),
                    new(ClaimTypes.Role, user.Role.ToString()) 
            };

            var identity = new ClaimsIdentity(claims,
               CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                }
            );
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
