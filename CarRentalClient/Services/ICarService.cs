using CarRentalClient.Services.Base;

namespace CarRentalClient.Services
{
    public interface ICarService
    {
        Task CreateCarAsync(Car car);
        Task DeleteCarAsync(int id);
        Task<Response<List<Car>>> GetAllCarsAsync();
        Task<Response<Car>> GetCarByIdAsync(int id);
        Task<Response<Car>> UpdateCarAsync(int id, Car car);
    }
}