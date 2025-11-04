using CarRental.Models;

namespace CarRental.Data
{
    public class CarRepository : ICar
    {
        private readonly ApplicationDbContext context;

        public CarRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Car car)
        {
            context.Cars.Add(car);
            context.SaveChanges();
        }

        public void Delete(Car car)
        {
            context.Cars.Remove(car);
            context.SaveChanges();
        }

        public IEnumerable<Car> GetAll()
        {
            return context.Cars.OrderBy(c => c.Model).ToList();
        }

        public Car GetById(int id)
        {
            return context.Cars.FirstOrDefault(c => c.Id == id);
        }

        public void Update(Car car)
        {
            context.Update(car);
            context.SaveChanges();
        }
    }
}
