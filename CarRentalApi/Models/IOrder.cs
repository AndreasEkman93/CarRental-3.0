using CarRental.Models;
using CarRentalApi.Dto;

namespace CarRental.Data
{
    public interface IOrder
    {
        IEnumerable<OrderDto> GetAll();
        IEnumerable<OrderDto> GetAllSpecificCustomer(string id);
        List<DateOnly> GetBookedDatesForCar(int carId);
        Order GetById(int id);
        OrderDto GetDtoById(int id);
        void Add(Order order);
        void Delete(Order order);
    }
}
