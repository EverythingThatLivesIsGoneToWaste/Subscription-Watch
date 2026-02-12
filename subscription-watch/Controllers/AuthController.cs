using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using subscription_watch.Exceptions;
using subscription_watch.Services;
using subscription_watch.DTOs;

namespace subscription_watch.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _authService.AuthenticateAsync(model.Login, model.Password);
                await _authService.LoginAsync(user);

                _logger.LogInformation("User {Login} logged in", user.Login);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Failed login attempt for {Login}", model.Login);
                ModelState.AddModelError(string.Empty, ex.Message);
                Response.StatusCode = 401;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Login}", model.Login);
                ModelState.AddModelError(string.Empty, "Internal server error");
                Response.StatusCode = 500;
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
