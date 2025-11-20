using CarRentalClient.Services.Base;

namespace CarRentalClient.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(LoginUserDto loginModel);
        public Task Logout();
    }
}
