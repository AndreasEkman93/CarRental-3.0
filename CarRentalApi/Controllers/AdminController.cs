using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("Customer")]
        public ActionResult<ApplicationUser> GetCustomer(string id)
        {
            var user = userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, ApplicationUser updatedUser)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpPost("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("AllCustomers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllCustomers()
        {
            var customers = await userManager.GetUsersInRoleAsync("Customer");
            return Ok(customers);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CreateUserViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Customer");
                return CreatedAtAction(nameof(GetCustomer), new { id = user.Id }, user);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


    }
}
