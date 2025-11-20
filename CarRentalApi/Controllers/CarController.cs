using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICar carRepository;

        public CarController(ICar carRepository)
        {
            this.carRepository = carRepository;
        }
        // GET: api/<CarController>
        [HttpGet]
        public ActionResult<IEnumerable<Car>> GetAll()
        {
            var cars = carRepository.GetAll();
            return Ok(cars);
        }

        // GET api/<CarController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult<Car> GetCar(int id)
        {
            var car = carRepository.GetById(id);
            if (car == null)
            {
                return NotFound();
            }
            return Ok(car);
        }

        // POST api/<CarController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<Car> PostCar(Car car)
        {
            if(car == null)
            {
                return BadRequest();
            }

            carRepository.Add(car);
            return Ok(car);
        }

        // PUT api/<CarController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Car> PutCar(int id, Car car)
        {
            if(car == null || car.Id != id)
            {
                return BadRequest();
            }
            try
            {
                carRepository.Update(car);
            }
            catch
            {
                return NotFound();
            }
            
            return Ok(car);
        }

        // DELETE api/<CarController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCar(int id)
        {
            var car = carRepository.GetById(id);
            if (car == null)
            {
                return NotFound();
            }
            try
            {
                carRepository.Delete(car);
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
