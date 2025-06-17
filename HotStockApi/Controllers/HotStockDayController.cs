using AutoMapper;
using EFCore.Data;
using EFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedProject.Models;
using System.Globalization;

namespace TTJApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotStockDayController(AppDbContext context, IMapper mapper, ILogger<HotStockDayController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotStockDayDto>>> GetDays()
    {
        var days = await context.HotStockDays
            .AsNoTracking()
            .Include(d => d.HotStockItems)
            .OrderByDescending(d => d.Date)
            .Take(10)
            .ToListAsync();

        return Ok(mapper.Map<List<HotStockDayDto>>(days));
    }

    [HttpPost]
    public async Task<ActionResult<HotStockDay>> PostDay(HotStockDayDto dayDto)
    {
        if (dayDto == null)
            return BadRequest("Invalid payload");

        var day = mapper.Map<HotStockDay>(dayDto);
        context.HotStockDays.Add(day);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDays), new { id = day.Id }, day);
    }

    [HttpPut("{date}")]
    public async Task<IActionResult> UpdateDay(string date, [FromBody] HotStockDayDto dayDto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            logger.LogInformation($"[UpdateDay] Received {dayDto.HotStockItems?.Count} items for {date}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var targetDate))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD");
            }

            var existingDay = await context.HotStockDays
                .Include(d => d.HotStockItems)
                .FirstOrDefaultAsync(d => d.Date.Date == targetDate.Date);

            if (existingDay == null)
                return NotFound($"Day with date {date} not found");

            if (existingDay.HotStockItems.Count == 0)
            {
                var newItems = mapper.Map<List<HotStockItem>>(dayDto.HotStockItems);

                foreach (var item in newItems)
                    item.HotStockDayId = existingDay.Id;

                await context.HotStockItems.AddRangeAsync(newItems);
            }

            existingDay.Summary = dayDto.Summary;
            existingDay.OpeningComment = dayDto.OpeningComment;
            existingDay.IsSummaryExpanded = dayDto.IsSummaryExpanded;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            logger.LogInformation($"[UpdateDay] Updated {dayDto.HotStockItems?.Count} items for {date}");
            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, $"[UpdateDay] Exception occurred for {date}");
            return StatusCode(500, "An error occurred while updating the day summary");
        }
    }
}