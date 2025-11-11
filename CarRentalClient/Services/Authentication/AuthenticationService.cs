using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

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

                //Decode the JWT to extract claims
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(response.Token) as JwtSecurityToken;

                //Extract roles from the token
                var roles = jsonToken?.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                    .Select(c => c.Value)
                    .ToList();

                Console.WriteLine($"Found {roles?.Count ?? 0} roles");

                //Store the token in session
                httpContextAccessor.HttpContext.Session.SetString("AccessToken", response.Token);

                //Store roles in session
                if (roles != null && roles.Any())
                {
                    httpContextAccessor.HttpContext.Session.SetString("UserRoles",
                        System.Text.Json.JsonSerializer.Serialize(roles));
                }

                return true;
            }

            return false;

        }

        public Task Logout()
        {
            httpContextAccessor.HttpContext.Session.Remove("AccessToken");
            httpContextAccessor.HttpContext.Session.Remove("UserRoles");
            return Task.CompletedTask;
        }
    }
}
