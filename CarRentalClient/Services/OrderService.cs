using System.Net.Http.Headers;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClient client;

        public OrderService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IClient client)
        {
            _httpContextAccessor = httpContextAccessor;
            this.client = client;
        }

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
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                await client.OrderDELETEAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete order.", ex);
            }
        }

        public async Task<IEnumerable<DateTimeOffset>> GetBookedDatesForCarAsync(int carId)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = new Response<List<DateTimeOffset>>()
            {
                Data = (await client.BookedDatesAsync(carId)).ToList(),
                Success = true
            };
            if (!response.Success)
            {
                throw new Exception("Failed to retrieve booked dates.");
            }
            return response.Data;
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
        }
    }
}
