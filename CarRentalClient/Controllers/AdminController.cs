using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly IOrder orderRepository; 
        private readonly ICar carRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(IOrder orderRepository, ICar carRepository, UserManager<ApplicationUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.carRepository = carRepository;
            this.userManager = userManager;
        }
        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            var orders = orderRepository.GetAll();
            var cars = carRepository.GetAll();
            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var adminVM = new AdminViewModel //Using a ViewModel to make a better index for Admins. 
            {
                Orders = orders,
                Cars = cars,
                Customers = customers
            };
            return View(adminVM);
        }

        // GET: AdminController/CustomerEdit/5
        public async Task<IActionResult> CustomerEdit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return View(user);
        }

        // POST: AdminController/CustomerEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerEdit(string id, ApplicationUser updatedUser)
        {
            try
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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was a problem when editing the ApplicationUser");
                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/CustomerDelete/5
        public async Task<IActionResult> CustomerDelete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return View(user);
        }

        // POST: AdminController/CustomerDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerDelete(string id, ApplicationUser applicationUser)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user.Id == applicationUser.Id)
                {
                    var result = await userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "There was a problem when removing the ApplicationUser");
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/CustomerCreate
        public IActionResult CustomerCreate()
        {
            return View();
        }

        // POST: AdminController/CustomerCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerCreate(CreateUserViewModel model) //ViewModel containing only the important properties
        {
                if (ModelState.IsValid)
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
                    await userManager.AddToRoleAsync(user, "Customer"); //If the user was correctly added set it's role to Customer. 
                        return RedirectToAction(nameof(Index));
                    }

                    foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                }
                return View(model);
        }
    }
}
