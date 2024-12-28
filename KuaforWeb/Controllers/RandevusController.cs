using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforWeb.Data;
using KuaforWeb.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using KuaforWeb.Data;

namespace KuaforWeb.Controllers
{
    [Authorize]
    public class RandevusController : Controller
    {
        private readonly AppDbContext _context;

        public RandevusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentTime = DateTime.UtcNow; // Use UTC time if your times are in UTC
            var upcomingRandevus = await _context.Randevus
                .Include(f => f.Salon) // Include the Salon data
                .Where(f => f.StartTime > currentTime) // Filter out overdue flights
                .ToListAsync();
            return View(upcomingRandevus);
        }


        // GET: Flights/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevus.FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // GET: Flights/Create
        [HttpGet]
        [Authorize(Policy = "AdminOrRedirect")]
        public IActionResult Create()
        {
            var salonList = _context.Salons.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.SalonId = new SelectList(salonList, "Id", "Name");
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrRedirect")]
        public async Task<IActionResult> Create(
            [Bind("RandevuNumber,StartTime,FinishTime,Origin,Price,SalonId")] Randevu randevu)
        {
            randevu.StartTime = randevu.StartTime.ToUniversalTime();
            randevu.FinishTime = randevu.FinishTime.ToUniversalTime();
            randevu.RandevuDuration = CalculateRandevuDuration(randevu.StartTime, randevu.FinishTime);
            randevu.RandevuStatus = "On-Time"; // Default status, adjust as necessary

            _context.Add(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Flights/Edit/{id}
        [HttpGet]
        [Authorize(Policy = "AdminOrRedirect")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevus.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // POST: Flights/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrRedirect")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,RandevuNumber,StartTime,FinishTime,Origin,Price")]
            Randevu randevu)
        {
            if (id != randevu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    randevu.RandevuDuration = CalculateRandevuDuration(randevu.StartTime, randevu.FinishTime);
                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RandevuExists(randevu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(randevu);
        }

        // POST: Flights/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrRedirect")]
        public async Task<IActionResult> Delete(int id)
        {
            var randevu = await _context.Randevus.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            _context.Randevus.Remove(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private TimeSpan CalculateRandevuDuration(DateTime startTime, DateTime finiishTime)
        {
            var duration = finiishTime - startTime;
            return TimeSpan.FromHours(Math.Round(duration.TotalHours));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevus.Any(e => e.Id == id);
        }
    }
}