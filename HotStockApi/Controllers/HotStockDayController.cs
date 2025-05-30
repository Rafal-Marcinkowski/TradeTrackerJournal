using EFCore.Data;
using EFCore.Models;
using HotStockTracker;
using HotStockTracker.MVVM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotStockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotStockDayController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<HotStockDayViewModel>> GetAll()
    {
        var days = await context.HotStockDays.Include(d => d.HotStockItems).ToListAsync();
        return days.Select(d => d.ToViewModel());
    }

    [HttpPost("day")]
    public async Task<ActionResult<HotStockDay>> PostDay(HotStockDay day)
    {
        foreach (var item in day.HotStockItems)
        {
            item.HotStockDay = day;
        }

        context.HotStockDays.Add(day);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = day.Id }, day);
    }
}
