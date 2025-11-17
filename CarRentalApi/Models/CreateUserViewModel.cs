using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Passwords don't match.")]
        public string? ConfirmPassword { get; set; }
    }
}
