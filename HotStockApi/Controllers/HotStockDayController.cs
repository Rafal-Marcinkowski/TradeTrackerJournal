using EFCore.Data;
using EFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotStockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotStockDayController : ControllerBase
{
    private readonly AppDbContext _context;
    public HotStockDayController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<HotStockDay>> GetAll()
    {
        return await _context.HotStockDays.Include(d => d.HotStockItems).ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Add(HotStockDay day)
    {
        _context.HotStockDays.Add(day);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = day.Id }, day);
    }
}
