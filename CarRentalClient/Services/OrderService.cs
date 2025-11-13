using System.Net.Http.Headers;
using CarRental.Models;
using CarRentalClient.Models;

namespace CarRentalClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/order");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve orders.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var orders = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Order>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return orders ?? new List<Order>();
        }

        public async Task<ApiResponse> CreateOrderAsync(OrderCreateViewModel model)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/order", model);
            if (response.IsSuccessStatusCode)
                return new ApiResponse(true, null);

            var error = await response.Content.ReadAsStringAsync();
            return new ApiResponse(false, error);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.DeleteAsync($"api/order/{id}");
        }

        public async Task<IEnumerable<DateOnly>> GetBookedDatesForCarAsync(int carId)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/order/booked-dates/{carId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve booked dates.");
            }
            var json = await response.Content.ReadAsStringAsync();
            var bookedDates = System.Text.Json.JsonSerializer.Deserialize<List<DateOnly>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return bookedDates ?? new List<DateOnly>();
        }
        
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/order/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve order.");
            }
            var json = await response.Content.ReadAsStringAsync();
            var order = System.Text.Json.JsonSerializer.Deserialize<Order>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return order!;
        }
    }
}
