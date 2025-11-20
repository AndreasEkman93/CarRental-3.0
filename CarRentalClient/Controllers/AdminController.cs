using CarRental.Models;
using CarRentalClient.Filters;
using CarRentalClient.Services;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ICarService carService;
        private readonly IAdminService adminService;

        public AdminController(IOrderService orderService, ICarService carService, IAdminService adminService)
        {
            this.orderService = orderService;
            this.carService = carService;
            this.adminService = adminService;
        }
        // GET: AdminController
        [JwtAuthorize("Admin")]
        public async Task<IActionResult> Index()
        {
            var ordersResponse = await orderService.GetOrdersAsync();
            var carsResponse = await carService.GetAllCarsAsync();
            var customersResponse = await adminService.GetAllCustomersAsync();
            var adminVM = new AdminViewModel
            {
                Orders = ordersResponse.Success ? ordersResponse.Data : new List<OrderDto>(),
                Cars = carsResponse.Success ? carsResponse.Data : new List<Car>(),
                Customers = customersResponse.Success ? customersResponse.Data : new List<ApplicationUser>()
            };
            return View(adminVM);
        }

        // GET: AdminController/CustomerEdit/5
        public async Task<IActionResult> CustomerEdit(string id)
        {
            var response = await adminService.GetCustomerAsync(id);
            if (!response.Success)
            {
                return NotFound();
            }
            return View(response.Data);
        }

        // POST: AdminController/CustomerEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerEdit(string id, ApplicationUser updatedUser)
        {
            var response = await adminService.UpdateCustomerAsync(id, updatedUser);
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "There was a problem when editing the ApplicationUser");
                return View(updatedUser);
            }
        }

        // GET: AdminController/CustomerDelete/5
        public async Task<IActionResult> CustomerDelete(string id)
        {
            var response = await adminService.GetCustomerAsync(id);
            if (!response.Success)
            {
                return NotFound();
            }
            return View(response.Data);
        }

        // POST: AdminController/CustomerDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerDelete(string id, ApplicationUser applicationUser)
        {
            var response = await adminService.DeleteCustomerAsync(id);
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "There was a problem when deleting the ApplicationUser");
                return View(applicationUser);
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
            var response = await adminService.CreateCustomerAsync(model);
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "There was a problem when creating the ApplicationUser");
                return View(model);
            }
        }
    }
}
