using AutoMapper;
using EFCore.Data;
using EFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedProject.Models;
using System.Diagnostics;

namespace HotStockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotStockDayController(AppDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotStockDayDto>>> GetDays()
    {
        var days = await context.HotStockDays
            .AsNoTracking()
            .Include(d => d.HotStockItems)
            .OrderByDescending(d => d.Date)
            .Take(5)
            .ToListAsync();

        foreach (var day in days)
        {
            Debug.WriteLine($"Day {day.Date} has {day.HotStockItems?.Count ?? 0} items in DB");
        }

        return Ok(mapper.Map<List<HotStockDayDto>>(days));
    }

    [HttpPost]
    public async Task<ActionResult<HotStockDay>> PostDay(HotStockDayDto dayDto)
    {
        Debug.WriteLine("POST method entered");
        if (dayDto == null)
        {
            Debug.WriteLine("dayDto is null!");
            return BadRequest("Invalid payload");
        }
        var day = new HotStockDay
        {
            Date = dayDto.Date,
            Summary = dayDto.Summary,
            IsSummaryExpanded = dayDto.IsSummaryExpanded,
            HotStockItems = mapper.Map<List<HotStockItem>>(dayDto.Items)
        };

        context.HotStockDays.Add(day);
        Debug.WriteLine($"Day has {day.HotStockItems.Count} items.");
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDays), new { id = day.Id }, day);
    }

    [HttpPut("{date}")]
    public async Task<IActionResult> UpdateDay(string date, [FromBody] HotStockDayDto dayDto)
    {
        if (!DateTime.TryParse(date, out var targetDate))
        {
            return BadRequest("Invalid date format. Use YYYY-MM-DD");
        }

        targetDate = targetDate.Date;

        var existingDay = await context.HotStockDays
            .FirstOrDefaultAsync(d => d.Date.Date == targetDate);

        if (existingDay == null)
        {
            Console.WriteLine($"Day not found for date: {targetDate:yyyy-MM-dd}");
            return NotFound();
        }

        existingDay.Summary = dayDto.Summary;
        existingDay.IsSummaryExpanded = dayDto.IsSummaryExpanded;

        try
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"Successfully updated day: {targetDate:yyyy-MM-dd}");
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating day: {ex}");
            return StatusCode(500, "Internal server error");
        }
    }
}
