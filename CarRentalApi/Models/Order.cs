using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
        public ApplicationUser Customer { get; set; }
        public string CustomerId { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
    }
}
