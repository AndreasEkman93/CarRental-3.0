using CarRentalClient.Services.Base;

namespace CarRentalClient.Services
{
    public class AdminService : IAdminService
    {
        private readonly IClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AdminService(IClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<ApplicationUser>> GetCustomerAsync(string id)
        {
            var token = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = new Response<ApplicationUser>();
            try
            {
                var user = await client.CustomerAsync(id);
                response.Data = user;
                response.Success = true;
            }
            catch (ApiException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<List<ApplicationUser>>> GetAllCustomersAsync()
        {
            var token = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = new Response<List<ApplicationUser>>();
            try
            {
                var users = await client.AllCustomersAsync();
                response.Data = users.ToList();
                response.Success = true;
            }
            catch (ApiException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<bool>> DeleteCustomerAsync(string id)
        {
            var token = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = new Response<bool>();
            try
            {
                await client.DeleteCustomerAsync(id);
                response.Data = true;
                response.Success = true;
            }
            catch (ApiException ex)
            {
                response.Data = false;
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<bool>> CreateCustomerAsync(CreateUserViewModel model)
        {
            var token = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = new Response<bool>();
            try
            {
                await client.CreateCustomerAsync(model);
                response.Data = true;
                response.Success = true;
            }
            catch (ApiException ex)
            {
                response.Data = false;
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<bool>> UpdateCustomerAsync(string id, ApplicationUser user)
        {
            var token = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = new Response<bool>();
            try
            {
                await client.UpdateCustomerAsync(id, user);
                response.Data = true;
                response.Success = true;
            }
            catch (ApiException ex)
            {
                response.Data = false;
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
