using Microsoft.AspNetCore.Mvc;
using KuaforWeb.Data;
using KuaforWeb.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SalonsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalonsController(AppDbContext context)
    {
        _context = context;
    }

    // Tüm salonları getirme
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Salon>>> GetSalons()
    {
        return await _context.Salons.ToListAsync();
    }

    // Belirli bir salonu ID'ye göre getirme
    [HttpGet("{id}")]
    public async Task<ActionResult<Salon>> GetSalon(int id)
    {
        var salon = await _context.Salons.FindAsync(id);

        if (salon == null)
        {
            return NotFound();
        }

        return salon;
    }

    // Yeni salon ekleme
    [HttpPost]
    public async Task<ActionResult<Salon>> PostSalon(Salon salon)
    {
        _context.Salons.Add(salon);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSalon), new { id = salon.Id }, salon);
    }

    // Salon güncelleme
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSalon(int id, Salon salon)
    {
        if (id != salon.Id)
        {
            return BadRequest();
        }

        _context.Entry(salon).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Salons.Any(e => e.Id == id))
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

    // Salon silme
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalon(int id)
    {
        var salon = await _context.Salons.FindAsync(id);
        if (salon == null)
        {
            return NotFound();
        }

        _context.Salons.Remove(salon);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
