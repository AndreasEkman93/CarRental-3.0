using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace CarRentalClient.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationService(IClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> AuthenticateAsync(LoginUserDto loginModel)
        {
            var response = await httpClient.LoginAsync(loginModel);

            if (response.Token != null && !string.IsNullOrEmpty(response.Token))
            {
                // Store the token in session
                httpContextAccessor.HttpContext.Session.SetString("AccessToken", response.Token);
                return true;
            }

            return false;

        }

        public Task Logout()
        {
            httpContextAccessor.HttpContext.Session.Remove("AccessToken");
            return Task.CompletedTask;
        }
    }
}
