using CarRental.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class OrderRepository : IOrder
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }

        public void Delete(Order order)
        {
            context.Orders.Remove(order);
            context.SaveChanges();
        }

        public IEnumerable<Order> GetAll()
        {
            return context.Orders.Include(o => o.Car).OrderBy(o => o.CustomerId).ThenByDescending(o=> o.EndDate).ToList();
        }

        public IEnumerable<Order> GetAllSpecificCustomer(string id)
        {
            return context.Orders.Include(o => o.Car).Where(o => o.CustomerId == id).OrderByDescending(o => o.StartDate).ToList();
        }

        public Order GetById(int id)
        {
            var order = context.Orders.Include(a => a.Customer).Include(b => b.Car).FirstOrDefault(s => s.Id==id);
            if (order != null)
            {
                return order;
            }
            return null;
        }

        public List<DateOnly> GetBookedDatesForCar(int carId)
        {
            var orders = context.Orders.Where(o => o.CarId==carId).ToList();

            var bookedDates = new List<DateOnly>();

            foreach (var order in orders)
            {
                for (var date = order.StartDate; date <= order.EndDate; date = date.AddDays(1))
                {
                    bookedDates.Add(date);
                }
            }
            return bookedDates;
        }
    }
}
