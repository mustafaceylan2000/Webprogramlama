using KuaforWeb.Data;
using KuaforWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaforWeb.Controllers
{
    [Authorize(Policy = "AdminOrRedirect")]
    public class SalonsController : Controller
    {
        private readonly AppDbContext _context;

        public SalonsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var salons = _context.Salons.ToList();
            return View(salons);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Salons == null)
            {
                return NotFound();
            }

            var salon = await _context.Salons
                .Include(a => a.Seats)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salon == null)
            {
                return NotFound();
            }

            // Calculate the count of available seats
            var availableSeatsCount = salon.Seats.Count(s => s.IsAvailable);

            // Pass this data to the view via ViewBag or a ViewModel
            ViewBag.AvailableSeatsCount = availableSeatsCount;

            return View(salon);
        }


        // GET: Airplanes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Airplanes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,SerialNumber")] Salon salon)
        {
            salon.NumberOfRows = 2; // Fixed number of rows
            salon.SeatsPerRow = 6; // Fixed seats per row

            _context.Add(salon);
            await _context.SaveChangesAsync();

            // Generate seats for the airplane
            GenerateSeatsForSalon(salon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void GenerateSeatsForSalon(Salon salon)
        {
            int totalRows = salon.NumberOfRows;
            int seatsPerRow = salon.SeatsPerRow;
            string[] columns = { "L", "L", "M", "M", "R", "R" }; // Column identifiers

            for (int row = 1;row <= totalRows;row++)
            {
                for (int seatNum = 1;seatNum <= seatsPerRow;seatNum++)
                {
                    string seatCode = $"{row}{columns[seatNum - 1]}{seatNum}";
                    Seat seat = new Seat
                    {
                        SalonId = salon.Id,
                        SeatCode = seatCode,
                        IsAvailable = true // Default class, can be modified as needed
                    };
                    _context.Seats.Add(seat);
                }
            }
        }


        // GET: Airplanes/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salon = await _context.Salons.FindAsync(id);
            if (salon == null)
            {
                return NotFound();
            }

            return View(salon);
        }

        // POST: Airplanes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
          [Bind("Id,Name,SerialNumber")] Salon salon)
        {
            if (id != salon.Id)
            {
                return NotFound();
            }

            _context.Update(salon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Airplanes/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var salon = await _context.Salons
                .Include(a => a.Seats) // Include associated seats
                .FirstOrDefaultAsync(a => a.Id == id);

            if (salon == null)
            {
                return NotFound();
            }

            // Remove the seats associated with the airplane
            foreach (var seat in salon.Seats)
            {
                _context.Seats.Remove(seat);
            }

            // Remove the airplane
            _context.Salons.Remove(salon);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}