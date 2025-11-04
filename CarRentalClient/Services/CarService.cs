using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Services
{
    public class CarService : ICarService
    {
        private readonly HttpClient httpClient;

        public CarService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            var response = await httpClient.GetAsync("https://localhost:7054/api/Car");
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
            var response = await httpClient.GetAsync($"https://localhost:7054/api/Car/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve car.");
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
            var response = await httpClient.PostAsJsonAsync("https://localhost:7054/api/Car", car);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create car.");
            }
        }
        public async Task UpdateCarAsync(int id, Car car)
        {
            var response = await httpClient.PutAsJsonAsync($"https://localhost:7054/api/Car/{id}", car);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update car.");
            }
        }

        public async Task DeleteCarAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"https://localhost:7054/api/Car/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete car.");
            }
        }
    }
}
