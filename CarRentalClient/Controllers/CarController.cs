using System.Threading.Tasks;
using CarRental.Data;
using CarRentalClient.Services;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace CarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService carService;

        public CarController(ICarService carService)
        {
            this.carService = carService;
        }

        // GET: CarController
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var cars = await carService.GetAllCarsAsync();
            return View(cars.Data);
        }

        // GET: CarController/Details/5
        //[Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult> Details(int id)
        {
            var car = await carService.GetCarByIdAsync(id);
            return View(car.Data);
        }

        // GET: CarController/Create
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        
        // POST: CarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }

            try
            {
                await carService.CreateCarAsync(car);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // Här kan du logga felet (t.ex. till konsol, fil, etc.)
                Console.WriteLine($"Error posting to API: {ex.Message}");
                ModelState.AddModelError("", "Ett oväntat fel uppstod vid kommunikation med API:t.");
            }

            return View(car);
        }

        // GET: CarController/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await carService.GetCarByIdAsync(id);
            return View(car);
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(Car car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }

            try
            {
                await carService.UpdateCarAsync(car.Id, car);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // Här kan du logga felet (t.ex. till konsol, fil, etc.)
                Console.WriteLine($"Error posting to API: {ex.Message}");
                ModelState.AddModelError("", "Ett oväntat fel uppstod vid kommunikation med API:t.");
            }

            return View(car);
        }

        // GET: CarController/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await carService.GetCarByIdAsync(id);
            return View(car);
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Car car)
        {
            if(car == null)
            {
                return BadRequest();
            }

            try
            {
                await carService.DeleteCarAsync(car.Id);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // Här kan du logga felet (t.ex. till konsol, fil, etc.)
                Console.WriteLine($"Error deleting from API: {ex.Message}");
                ModelState.AddModelError("", "Ett oväntat fel uppstod vid kommunikation med API:t.");
            }
            return View(car);
        }
    }
}
