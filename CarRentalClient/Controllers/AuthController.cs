using System.Net.Http;
using CarRentalClient.Services.Base;
using CarRentalClient.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IClient httpClient;
        private readonly Services.Authentication.IAuthenticationService authenticationService;

        public AuthController(IClient httpClient, IAuthenticationService authenticationService)
        {
            this.httpClient = httpClient;
            this.authenticationService = authenticationService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
                return View(userDto);

            try
            {
                await httpClient.RegisterAsync(userDto);
                return RedirectToAction("Login", "Auth");
            }
            catch (ApiException ex)
            {
                // Om du använder NSwag/Refit: fånga API-fel här
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(userDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(userDto);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {

            if (!ModelState.IsValid)
                return View(loginUserDto);
            try
            {
                var success = await authenticationService.AuthenticateAsync(loginUserDto);

                if (success)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Credentials, Try again!");
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode >= 200 && ex.StatusCode <= 299)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ex.Response);
            }
            return View(loginUserDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await authenticationService.Logout();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
