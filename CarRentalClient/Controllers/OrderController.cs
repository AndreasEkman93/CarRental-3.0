using CarRental.Data;
using CarRental.Models;
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

        public async Task<ActionResult> Index()
        {
            var orders = await _orderService.GetOrdersAsync();
            return View(orders);
        }

        public async Task<ActionResult> Create(int id)
        {
            var model = new OrderCreateViewModel
            {
                CarId = id,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };
            var car = await carService.GetCarByIdAsync(id);
            ViewBag.CarModel = car.Model;

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
                    ViewBag.CarModel = carService.GetCarByIdAsync(model.CarId).Result.Model;
                    return View();
                }
                var orders = await _orderService.GetOrdersAsync();
                var existingOrders = orders
                    .Where(o => o.CarId == model.CarId)
                    .ToList();

                foreach (var existingOrder in existingOrders)
                {
                    if (model.StartDate <= existingOrder.EndDate && model.EndDate >= existingOrder.StartDate)
                    {
                        ModelState.AddModelError("", "Selected dates overlap with an existing booking.");
                        ViewBag.CarModel = carService.GetCarByIdAsync(model.CarId).Result.Model;
                        var bookedDates = await _orderService.GetBookedDatesForCarAsync(model.CarId);
                        ViewBag.BookedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
                        return View(model);
                    }
                }

                var result = await _orderService.CreateOrderAsync(model);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(model);
                }

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
            return View(order);
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }
            try
            {
                await _orderService.DeleteOrderAsync(order.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Här kan du logga felet (t.ex. till konsol, fil, etc.)
                Console.WriteLine($"Error deleting from API: {ex.Message}");
                ModelState.AddModelError("", "Ett oväntat fel uppstod vid kommunikation med API:t.");
            }
            return View(order);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            var car = await carService.GetCarByIdAsync(order.CarId);

            ViewBag.CarModel = car.Model;
            ViewBag.CarRegNr = car.RegNr;
            return View(order);
        }
    }
}
