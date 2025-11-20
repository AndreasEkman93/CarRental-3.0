using Microsoft.AspNetCore.Identity;

namespace CarRental.Models
{
    public class ApplicationUser:IdentityUser
    {
        public List<Order>? Orders { get; set; }
        public string? Name { get; set; }
    }
}
