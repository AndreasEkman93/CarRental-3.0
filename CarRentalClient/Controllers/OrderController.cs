using CarRentalClient.Services.Base;
using CarRentalClient.Filters;
using CarRentalClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarRental.Controllers
{
    [JwtAuthorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICarService carService;

        public OrderController(IOrderService orderService, ICarService carService)
        {
            _orderService = orderService;
            this.carService = carService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _orderService.GetOrdersAsync();

            if (!response.Success)
            {
                ViewBag.ErrorMessage = response.Message ?? "Kunde inte hämta ordrar.";
                return View(new List<OrderDto>()); 
            }
            return View(response.Data);
        }

        [HttpGet]
        [JwtAuthorize("Customer")]
        public async Task<ActionResult> Create(int id)
        {
            var model = new OrderCreateViewModel
            {
                CarId = id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            var car = await carService.GetCarByIdAsync(id);
            ViewBag.CarModel = car.Data.Model;

            // Get all booked dates for this car to help the customer not to pick reserved dates
            var bookedDates = await _orderService.GetBookedDatesForCarAsync(id);
            ViewBag.BookedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            try
            {
                if (model.EndDate < model.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be after or equal to Start Date.");
                    ViewBag.CarModel = carService.GetCarByIdAsync(model.CarId).Result.Data.Model;
                    return View();
                }
                var orders = await _orderService.GetOrdersAsync();
                var existingOrders = orders
                    .Data
                    .Where(o => o.CarId == model.CarId)
                    .ToList();

                foreach (var existingOrder in existingOrders)
                {
                    if (model.StartDate <= existingOrder.EndDate && model.EndDate >= existingOrder.StartDate)
                    {
                        ModelState.AddModelError("", "Selected dates overlap with an existing booking.");
                        ViewBag.CarModel = carService.GetCarByIdAsync(model.CarId).Result.Data.Model;
                        var bookedDates = await _orderService.GetBookedDatesForCarAsync(model.CarId);
                        ViewBag.BookedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
                        return View(model);
                    }
                }

                await _orderService.CreateOrderAsync(model);
                return RedirectToAction("OrderConfirmation");
            }
            catch
            {
                return View();
            }
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        // GET: OrderController/Delete/5

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View(order.Data);
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest();
            }
            try
            {
                await _orderService.DeleteOrderAsync(orderDto.Id);

                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Account");

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                //Get role from token
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                if (role == "Admin")
                {
                    return RedirectToAction(nameof(AdminController.Index), "Admin");
                }
                else if (role == "Customer")
                {
                    return RedirectToAction(nameof(OrderController.Index), "Order");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting from API: {ex.Message}");
                ModelState.AddModelError("", "Ett oväntat fel uppstod vid kommunikation med API:t.");
            }
            return View(orderDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            var car = await carService.GetCarByIdAsync(order.Data.CarId);

            ViewBag.CarModel = car.Data.Model;
            ViewBag.CarRegNr = car.Data.RegNr;
            return View(order.Data);
        }
    }
}
