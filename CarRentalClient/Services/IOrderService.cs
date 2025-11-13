using CarRentalClient.Services.Base;

namespace CarRentalClient.Services
{
    public interface IOrderService
    {
        Task<Response<List<Order>>> GetOrdersAsync();
        Task CreateOrderAsync(OrderCreateViewModel model);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<DateOnly>> GetBookedDatesForCarAsync(int carId);
        Task<Order> GetOrderByIdAsync(int id);
    }
}
