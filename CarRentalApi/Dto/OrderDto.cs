namespace CarRentalApi.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarModel { get; set; }
        public string CustomerId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
