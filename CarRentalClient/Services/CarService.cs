using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Services
{
    public class CarService : ICarService
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CarService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToRequestHeaders()
        {
            var token = httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            Console.WriteLine($"Token in session: {token}");
            httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            AddJwtToRequestHeaders();
            var response = await httpClient.GetAsync("/api/Car");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve cars.");
            }
            var json = await response.Content.ReadAsStringAsync();
            var cars = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Car>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return cars;
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {

            AddJwtToRequestHeaders();
            var response = await httpClient.GetAsync($"/api/Car/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve car errorCode:{response.StatusCode}.");
            }
            var json = await response.Content.ReadAsStringAsync();
            var car = System.Text.Json.JsonSerializer.Deserialize<Car>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return car;
        }

        public async Task CreateCarAsync(Car car)
        {

            AddJwtToRequestHeaders();
            var response = await httpClient.PostAsJsonAsync("/api/Car", car);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create car.");
            }
        }
        public async Task UpdateCarAsync(int id, Car car)
        {

            AddJwtToRequestHeaders();
            var response = await httpClient.PutAsJsonAsync($"/api/Car/{id}", car);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update car.");
            }
        }

        public async Task DeleteCarAsync(int id)
        {

            AddJwtToRequestHeaders();
            var response = await httpClient.DeleteAsync($"/api/Car/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete car.");
            }
        }
    }
}
