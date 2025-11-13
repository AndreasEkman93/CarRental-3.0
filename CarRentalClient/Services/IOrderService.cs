using CarRental.Models;

using CarRentalClient.Models;

namespace CarRentalClient.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<ApiResponse> CreateOrderAsync(OrderCreateViewModel model);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<DateOnly>> GetBookedDatesForCarAsync(int carId);
        Task<Order> GetOrderByIdAsync(int id);
    }
}
