using CarRentalClient.Services.Base;

namespace CarRentalClient.Services
{
    public interface IAdminService
    {
        Task<Response<bool>> CreateCustomerAsync(CreateUserViewModel model);
        Task<Response<bool>> DeleteCustomerAsync(string id);
        Task<Response<List<ApplicationUser>>> GetAllCustomersAsync();
        Task<Response<ApplicationUser>> GetCustomerAsync(string id);
        Task<Response<bool>> UpdateCustomerAsync(string id, ApplicationUser user);
    }
}