using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeRental;
using BikeRental.Model;

namespace BikeRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly BikeRentalContext _context;

        public RentalsController(BikeRentalContext context)
        {
            _context = context;
        }

        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentals()
        {
            return await _context.Rentals.Include(r => r.Customer).Include(b => b.Bike).ToListAsync();
        }

        // GET: api/Rentals/Unpaid
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetUnpaidRentals()
        {
            //TODO  Get a list of unpaid, ended rentals with total price > 0. For each rental, 
            //      the following data must be returned:
            //      Customer's ID, first and last name
            //      Rental's ID, start end, end date, and total price
            var rentals = await _context.Rentals.Include(r => r.Customer)
                            .Where(r => r.Paid == false && r.TotalCost > 0)
                            .ToListAsync();
            return rentals; // NO
        }

        // GET: api/Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // PUT: api/Rentals/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> EndRental(int id, Rental rental)
        {
            if (id != rental.RentalId)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                rental.RentalEnd = DateTime.Now;
                rental.TotalCost = CalcTotalCost(rental);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Function to calculate the total cost of one rental
        /// </summary>
        /// <param name="rental"></param>
        /// <returns>totalCost</returns>
        private decimal CalcTotalCost(Rental rental)
        {
            int duration = rental.RentalEnd.Millisecond - rental.RentalBegin.Millisecond;
            decimal totalCost = 0;
            if (duration <= 15 * 60 * 1000)
            {
                return totalCost;
            }
            totalCost += rental.Bike.PriceFirstHour;
            for (duration -= 60 * 60 * 1000; duration > 0; duration -= 60 * 60 * 1000)
            {
                totalCost += rental.Bike.PriceAdditionalHours;
            }
            return totalCost;
        }

        // POST: api/Rentals
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Rental>> PostRental(Rental rental)
        {
            if (rental.Customer.Rentals.Last().RentalEnd == null && rental.Customer.Rentals.Last().TotalCost <= 0)
            {
                throw MultipleRentalsException(); // Should probably throw an exception or something instead of this
            }
            if (rental.TotalCost != 0 || rental.RentalEnd != null)
            {
                throw InvalidPramaterException();
            }
            rental.RentalBegin = DateTime.Now;
            rental.TotalCost = -1;
            _context.Rentals.Add(rental);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRental", new { id = rental.RentalId }, rental);
        }

        // Put: api/Rental/5/Pay
        // Set Paid flag on true
        [HttpPut]
        [Route("{id}/Pay")]
        public async Task<IActionResult> PayRental(int id, Rental rental)
        {
            if (id != rental.RentalId)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                if (rental.TotalCost > 0)
                {
                    rental.Paid = true;
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }





        // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rental>> DeleteRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return rental;
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }

        // Exceptions
        private Exception InvalidPramaterException() // RentalEnd and TotalCost must be empty
        {
            throw new NotImplementedException();
        }
        private Exception MultipleRentalsException() // A customer can only have 1 active rental
        {
            throw new NotImplementedException();
        }
    }
}
