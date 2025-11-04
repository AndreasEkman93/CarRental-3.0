namespace CarRental.Models
{
    public class AdminViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Car> Cars { get; set; }
        public IEnumerable<ApplicationUser> Customers { get; set; }

    }
}
