using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalClient.Services
{
    public class CarService : ICarService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IClient client;

        public CarService(IHttpContextAccessor httpContextAccessor, IClient client)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.client = client;
        }

        private void AddJwtToRequestHeaders()
        {
            var token = httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
                        client.HttpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                client.HttpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<Response<List<Car>>> GetAllCarsAsync()
        {
            AddJwtToRequestHeaders();
            Response<List<Car>> response;
            var data = await client.CarAllAsync();
            response = new Response<List<Car>>
            {
                Data = data.ToList(),
                Success = true
            };

            return response;
        }

        public async Task<Response<Car>> GetCarByIdAsync(int id)
        {

            AddJwtToRequestHeaders();
            Response<Car> response;
            var data = await client.CarGETAsync(id);
            response = new Response<Car>
            {
                Data = data,
                Success = true
            };
            return response;

        }

        public async Task CreateCarAsync(Car car)
        {

            AddJwtToRequestHeaders();
            try
            {
                await client.CarPOSTAsync(car);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create car.", ex);
            }
        }

        public async Task UpdateCarAsync(int id, Car car)
        {

            AddJwtToRequestHeaders();
            //var response = await httpClient.PutAsJsonAsync($"/api/Car/{id}", car);
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception("Failed to update car.");
            //}
            try
            {
                await client.CarPUTAsync(id, car);
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to update car.", ex);
            }
        }

        public async Task DeleteCarAsync(int id)
        {

            AddJwtToRequestHeaders();
            //var response = await httpClient.DeleteAsync($"/api/Car/{id}");
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception("Failed to delete car.");
            //}
            try
            {
                await client.CarDELETEAsync(id);
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to delete car.", ex);
            }
        }
    }
}
