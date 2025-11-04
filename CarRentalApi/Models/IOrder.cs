using CarRental.Models;

namespace CarRental.Data
{
    public interface IOrder
    {
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetAllSpecificCustomer(string id);
        List<DateOnly> GetBookedDatesForCar(int carId);
        Order GetById(int id);
        void Add(Order order);
        void Delete(Order order);
    }
}
