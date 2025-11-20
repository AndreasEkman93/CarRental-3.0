using System.ComponentModel.DataAnnotations;

namespace CarRentalShared.Dto
{
    public class UserDto:LoginUserDto
    {

        [Required]
        public string Name { get; set; }
    }
}