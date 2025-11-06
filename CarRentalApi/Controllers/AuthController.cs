using CarRental.Models;
using CarRentalApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    Name = userDto.Name,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded == false)
                {
                    foreach (var item in result.Errors)
                    {
                        {
                            ModelState.AddModelError(item.Code, item.Description);
                        }
                        return BadRequest(ModelState);
                    }
                }
                await userManager.AddToRoleAsync(user, "Customer");
                return Accepted();
            }
            catch (Exception ex)
            {
                return Problem($"Something Went Wrong in the{ex}", statusCode:500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await userManager.CheckPasswordAsync(user, userDto.Password);
               
                if (user == null || passwordValid == false)
                {
                    return NotFound();
                }

                //Add whatever is needed to create a JWT

                return Accepted();
            }
            catch (Exception ex)
            {
                return Problem($"Something Went Wrong in the{ex}", statusCode: 500);
            }
        }
    }
}
