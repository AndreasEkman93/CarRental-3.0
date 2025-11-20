using CarRental.Models;
using CarRentalApi.Dto;
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

        public IEnumerable<OrderDto> GetAll()
        {
            return context.Orders.Include(c => c.Customer).Include(o => o.Car).OrderBy(o => o.CustomerId).ThenByDescending(o => o.EndDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CarId = o.CarId,
                    CarModel = o.Car.Model,
                    CustomerId = o.CustomerId,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate
                }).ToList();
        }

        public IEnumerable<OrderDto> GetAllSpecificCustomer(string id)
        {

            return context.Orders
                .Include(o => o.Car)
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.StartDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CarId = o.CarId,
                    CarModel = o.Car.Model,
                    CustomerId = o.CustomerId,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate
                })
                .ToList();
        }
        public Order GetById(int id)
        {
            var order = context.Orders.Include(a => a.Customer).Include(b => b.Car).FirstOrDefault(s => s.Id == id);
            if (order != null)
            {
                return order;
            }
            return null;
        }
        public OrderDto GetDtoById(int id)
        {
            var order = context.Orders
                .Include(a => a.Customer)
                .Include(b => b.Car)
                .FirstOrDefault(s => s.Id == id);
            if (order != null)
            {
                return new OrderDto
                {
                    Id = order.Id,
                    CarId = order.CarId,
                    CarModel = order.Car.Model,
                    CustomerId = order.CustomerId,
                    StartDate = order.StartDate,
                    EndDate = order.EndDate
                };
            }
            return null;
        }

        public List<DateOnly> GetBookedDatesForCar(int carId)
        {
            var orders = context.Orders.Where(o => o.CarId == carId).ToList();

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
