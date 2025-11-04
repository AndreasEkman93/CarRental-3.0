using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string RegNr { get; set; }
        [Required]
        public int PricePerDay { get; set; }
        [Required]
        public List<String> ImageUrls { get; set; }
    }
}
