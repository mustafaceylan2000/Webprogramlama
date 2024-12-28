using KuaforWeb.Data;
using KuaforWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforWeb.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public TicketsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id; // Retrieve the unique Id of the logged-in user

            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Randevu)
                .Include(t => t.Seat)
                .ToListAsync();
            return View(tickets);
        }

        // GET: Tickets/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Randevu)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Randevu)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Check if the ticket can be cancelled (more than 2 hours before flight)
            if (ticket.Randevu.StartTime.Subtract(DateTime.Now).TotalHours <= 2)
            {
                ModelState.AddModelError("", "Cannot cancel ticket within 2 hours of randevu starttime.");
                return View("Details", ticket);
            }

            // Cancel the ticket
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Additional methods and actions...
    }
}
