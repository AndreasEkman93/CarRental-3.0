using System.ComponentModel.DataAnnotations;

namespace CarRentalApi.Dto
{
    public class UserDto:LoginUserDto
    {

        [Required]
        public string Name { get; set; }
    }
}