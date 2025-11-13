using System.Net.Http.Headers;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClient client;

        public OrderService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IClient client)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            this.client = client;
        }

        //public async Task<Response<List<Order>>> GetOrdersAsync()
        //{
        //    var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
        //    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    Response<List<Order>> response;
        //    //var response = await _httpClient.GetAsync("api/order");
        //    var data = await client.OrderAllAsync();

        //    response = new Response<List<Order>>
        //    {
        //        Data = data.ToList(),
        //        Success = true
        //    };
        //    return response;
        //}
        public async Task<Response<List<OrderDto>>> GetOrdersAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Response<List<OrderDto>> response;
            var data = await client.OrderAllAsync();
            response = new Response<List<OrderDto>>
            {
                Data = data.ToList(),
                Success = true
            };
            return response;
        }

        public async Task CreateOrderAsync(OrderCreateViewModel model)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var response = await _httpClient.PostAsJsonAsync("api/order", model);
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception("Failed to create order.");
            //}
            try
            {
                await client.OrderPOSTAsync(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create order.", ex);
            }
            

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
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        
        public async Task<Response<OrderDto>> GetOrderByIdAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Response<OrderDto> response;
            var data = await client.OrderGETAsync(id);
            response = new Response<OrderDto>
            {
                Data = data,
                Success = true
            };
            return response;

            //var response = await _httpClient.GetAsync($"api/order/{id}");
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception("Failed to retrieve order.");
            //}
            //var json = await response.Content.ReadAsStringAsync();
            //var order = System.Text.Json.JsonSerializer.Deserialize<OrderDto>(json, new System.Text.Json.JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //});
            //return order!;
        }
    }
}
