namespace CarRental.Models
{
    public class OrderCreateViewModel
    {
        public int CarId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
