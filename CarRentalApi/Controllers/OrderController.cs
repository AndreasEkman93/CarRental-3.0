using CarRental.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using CarRentalApi.Dto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarRentalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrder orderRepository;
        private readonly ICar carRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderController(IOrder orderRepository, ICar carRepository, UserManager<ApplicationUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.carRepository = carRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (User.IsInRole("Admin"))
                return Ok(orderRepository.GetAll());
            else
                return Ok(orderRepository.GetAllSpecificCustomer(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<OrderDto> GetOrderById(int id)
        {
            var order = orderRepository.GetById(id);
                if(order == null)
            {
                return NotFound();
            }

                var dto = new OrderDto
                {
                    Id = order.Id,
                    CarId = order.CarId,
                    CarModel = order.Car.Model,
                    CustomerId = order.CustomerId,
                    StartDate = order.StartDate,
                    EndDate = order.EndDate
                };
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult PostOrder(OrderCreateViewModel model)
        {
            if(model == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirst("uid")?.Value;
            var order = new Order
            {
                CarId = model.CarId,
                CustomerId = userId,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            orderRepository.Add(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteOrder(int id)
        {
            var order = orderRepository.GetById(id);
            if (order == null)
                return NotFound();

            orderRepository.Delete(order);
            return NoContent();
        }

        [HttpGet("booked-dates/{carId}")]
        public ActionResult<List<DateOnly>> GetBookedDatesForCar(int carId)
        {
            var bookedDates = orderRepository.GetBookedDatesForCar(carId);
            return Ok(bookedDates);
        }
    }
}
