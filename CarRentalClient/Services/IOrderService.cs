using CarRentalClient.Services.Base;

namespace CarRentalClient.Services
{
    public interface IOrderService
    {
        Task<Response<List<OrderDto>>> GetOrdersAsync();
        Task CreateOrderAsync(OrderCreateViewModel model);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<DateOnly>> GetBookedDatesForCarAsync(int carId);
        Task<Response<OrderDto>> GetOrderByIdAsync(int id);
    }
}
