using CarRental.Models;

namespace CarRentalClient.Services
{
    public interface ICarService
    {
        Task CreateCarAsync(Car car);
        Task DeleteCarAsync(int id);
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task UpdateCarAsync(int id, Car car);
    }
}