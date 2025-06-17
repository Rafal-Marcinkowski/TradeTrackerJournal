using AutoMapper;
using EFCore.Data;
using EFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockNotepad.MVVM.Models;

namespace TTJApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotepadCompanyItemController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]

    public async Task<ActionResult<List<NotepadCompanyItemDto>>> Get()
    {
        var entities = await db.CompanyItems
            .Include(x => x.Summary)
            .Include(x => x.Notes)
            .ToListAsync();

        var dtos = mapper.Map<List<NotepadCompanyItemDto>>(entities);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NotepadCompanyItemDto dto)
    {
        var entity = mapper.Map<NotepadCompanyItem>(dto);
        db.CompanyItems.Add(entity);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] NotepadCompanyItemDto dto)
    {
        var existing = await db.CompanyItems
            .Include(x => x.Summary)
            .Include(x => x.Notes)
            .FirstOrDefaultAsync(x => x.CompanyName == dto.CompanyName);

        if (existing == null) return NotFound();

        existing.Summary = mapper.Map<CompanySummary>(dto.Summary);
        existing.Notes = mapper.Map<List<Note>>(dto.Notes);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("note/{noteId}")]
    public async Task<IActionResult> DeleteNote(int noteId)
    {
        var note = await db.CompanyNotes.FindAsync(noteId);
        if (note == null)
            return NotFound();

        db.CompanyNotes.Remove(note);
        await db.SaveChangesAsync();

        return Ok();
    }
}