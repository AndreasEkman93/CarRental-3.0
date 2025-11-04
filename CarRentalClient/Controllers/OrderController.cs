using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarRental.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrder orderRepository;
        private readonly ICar carRepository;
        private readonly UserManager<ApplicationUser> userManager; //Used to get current logged in user.

        public OrderController(IOrder orderRepository, ICar carRepository,UserManager<ApplicationUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.carRepository = carRepository;
            this.userManager = userManager;
        }
        // GET: OrderController
        
        public ActionResult Index()
        {
            var userId = userManager.GetUserId(User);
            if (User.IsInRole("Admin")){ //Depending on role it will generate different lists.
                return View(orderRepository.GetAll());
            }
            else
                return View(orderRepository.GetAllSpecificCustomer(userId));
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            return View(orderRepository.GetById(id));
        }

        // GET: OrderController/Create
        [Authorize(Roles ="Customer")]
        public ActionResult Create(int id)
        {
            var orderCreateVM = new OrderCreateViewModel()  
            {
                CarId = id,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };
            Car car = carRepository.GetById(id);
            ViewBag.CarModel = car.Model;

            // Get all booked dates for this car to help the customer not to pick reserved dates
            var bookedDates = orderRepository.GetBookedDatesForCar(id);
            ViewBag.BookedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();

            return View(orderCreateVM);
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create(OrderCreateViewModel model)
        {
            try
            {
                if (model.EndDate < model.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be after or equal to Start Date.");
                    ViewBag.CarModel = carRepository.GetById(model.CarId).Model;
                    return View();
                }
                
                var existingOrders = orderRepository.GetAll()
                    .Where(o => o.CarId == model.CarId)
                    .ToList();

                foreach (var existingOrder in existingOrders)
                {
                    if(model.StartDate <= existingOrder.EndDate && model.EndDate >= existingOrder.StartDate)
                    {
                        ModelState.AddModelError("", "Selected dates overlap with an existing booking.");
                        ViewBag.CarModel = carRepository.GetById(model.CarId).Model;
                        var bookedDates = orderRepository.GetBookedDatesForCar(model.CarId);
                        ViewBag.BookedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
                        return View(model);
                    }
                }

                if (ModelState.IsValid)
                {
                    var userId = userManager.GetUserId(User);
                    var order = new Order
                    {
                        CarId = model.CarId,
                        CustomerId = userId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    
                    orderRepository.Add(order);
                }
                return RedirectToAction("OrderConfirmation");
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(orderRepository.GetById(id));
        }
        
        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Order order)
        {
            try
            {
                orderRepository.Delete(order);
                
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index","Admin");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
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

    }
}
