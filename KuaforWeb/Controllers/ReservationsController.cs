using KuaforWeb.Data;
using KuaforWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using KuaforWeb.Data;
//using KuaforWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforWeb.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id; // Retrieve the unique Id of the logged-in user

            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Randevu)
                .Include(r => r.Seat)
                .ToListAsync();
            return View(reservations);
        }

        // GET: Reservations/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Filter to include only upcoming flights
            var upcomingRandevus = _context.Randevus
                .Where(f => f.StartTime > DateTime.UtcNow)
                .Select(f => new { f.Id, RandevuDetails = f.RandevuNumber });

            ViewData["RandevuId"] = new SelectList(upcomingRandevus, "Id", "RandevuDetails");
            ViewData["SeatId"] = new SelectList(_context.Seats.Where(s => s.IsAvailable), "Id", "SeatCode");

            return View();
        }


        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RandevuId,SeatId")] Reservation reservation)
        {
            // Load the corresponding Flight and Seat from the database
            var randevu = await _context.Randevus.FindAsync(reservation.RandevuId);
            var seat = await _context.Seats.FindAsync(reservation.SeatId);

            if (randevu == null || seat == null)
            {
                // Flight or Seat not found, add an error message and return to the form
                ModelState.AddModelError("", "Randevus or Seat not found.");
                ViewData["RandevuId"] = new SelectList(_context.Randevus, "Id", "RandevuNumber", reservation.RandevuId);
                ViewData["SeatId"] = new SelectList(_context.Seats.Where(s => s.IsAvailable), "Id", "SeatCode",
                    reservation.SeatId);
                return View(reservation);
            }

            // Set additional properties of the Reservation
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                reservation.UserId = user.Id; // Set the user ID from UserManager
            }
            else
            {
                // Handle the case where the user is not found
                ModelState.AddModelError("", "User not found.");
                return View(reservation);
            }

            reservation.ReservationDate = DateTime.UtcNow; // Set the reservation date to current UTC time

            if (seat.IsAvailable)
            {
                seat.IsAvailable = false;
                _context.Update(seat);
            }
            else
            {
                ModelState.AddModelError("", "The selected seat is no longer available.");
                ViewData["RandevuId"] = new SelectList(_context.Randevus, "Id", "RandevuNumber", reservation.RandevuId);
                ViewData["SeatId"] = new SelectList(_context.Seats.Where(s => s.IsAvailable), "Id", "SeatCode",
                    reservation.SeatId);
                return View(reservation);
            }

            // Add the Reservation to the context and save changes
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Reservations/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Randevu)
                .Include(r => r.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["RandevuId"] = new SelectList(_context.Randevus, "Id", "RandevuNumber", reservation.RandevuId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "SeatCode", reservation.SeatId);
            return View(reservation);
        }


        // POST: Reservations/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RandevuId,SeatId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            var existingReservation = await _context.Reservations
                .Include(r => r.Seat)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReservation == null)
            {
                return NotFound();
            }

            try
            {
                // Update the SeatId if changed and ensure the selected seat is available
                if (existingReservation.SeatId != reservation.SeatId)
                {
                    var newSeat = await _context.Seats.FindAsync(reservation.SeatId);
                    if (newSeat == null || !newSeat.IsAvailable)
                    {
                        // Add error message if new seat is not available
                        ModelState.AddModelError("SeatId", "The selected seat is not available.");
                        ViewData["RandevuId"] =
                            new SelectList(_context.Randevus, "Id", "RandevuNumber", reservation.RandevuId);
                        ViewData["SeatId"] = new SelectList(_context.Seats.Where(s => s.IsAvailable), "Id", "SeatCode",
                            reservation.SeatId);
                        return View(reservation);
                    }

                    // Mark the old seat as available
                    var oldSeat = existingReservation.Seat;
                    if (oldSeat != null)
                    {
                        oldSeat.IsAvailable = true;
                        _context.Update(oldSeat);
                    }

                    // Update to new seat and mark as unavailable
                    newSeat.IsAvailable = false;
                    existingReservation.SeatId = newSeat.Id;
                    _context.Update(newSeat);
                }

                // Update the ReservationDate to current UTC time
                existingReservation.ReservationDate = DateTime.UtcNow;

                // Save changes
                _context.Update(existingReservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to Index action after successful update
            return RedirectToAction(nameof(Index));
        }


        // POST: Reservations/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Reservations/Purchase/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Randevu)
                .Include(r => r.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Creating a ticket from the reservation
            var ticket = new Ticket
            {
                UserId = reservation.UserId,
                RandevuId = reservation.RandevuId,
                SeatId = reservation.SeatId,
                PurchaseDate = DateTime.UtcNow, // Use UtcNow instead of Now
                TicketNumber = GenerateTicketNumber() // Implement this method as needed
            };

            _context.Tickets.Add(ticket);

            // Deleting the reservation
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        private string GenerateTicketNumber()
        {
            // Implement ticket number generation logic
            return "TICKET-" + Guid.NewGuid().ToString();
        }
    }
}