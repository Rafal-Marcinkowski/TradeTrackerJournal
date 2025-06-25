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
    public async Task<ActionResult<IEnumerable<NotepadCompanyItemDto>>> Get()
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
        if (entity.Summary != null)
            entity.Summary.NotepadCompanyItem = entity;
        db.CompanyItems.Add(entity);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}/summary")]
    public async Task<IActionResult> UpdateSummary(int id, [FromBody] CompanySummaryDto summaryDto)
    {
        var entity = await db.CompanyItems
            .Include(x => x.Summary)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return NotFound();

        if (entity.Summary == null)
        {
            entity.Summary = mapper.Map<CompanySummary>(summaryDto);
            entity.Summary.NotepadCompanyItemId = id;
            db.CompanySummaries.Add(entity.Summary);
        }
        else
        {
            mapper.Map(summaryDto, entity.Summary);
            entity.Summary.UpdatedAt = DateTime.Now;
        }

        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id}/notes")]
    public async Task<IActionResult> AddNote(int id, [FromBody] NoteDto noteDto)
    {
        var exists = await db.CompanyItems.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        var note = mapper.Map<Note>(noteDto);
        note.NotepadCompanyItemId = id;
        db.CompanyNotes.Add(note);
        await db.SaveChangesAsync();
        return Ok(note.Id);
    }

    [HttpPut("/notes/{noteId}")]
    public async Task<IActionResult> UpdateNote(int noteId, [FromBody] NoteDto dto)
    {
        var note = await db.CompanyNotes.FindAsync(noteId);
        if (note == null) return NotFound();

        mapper.Map(dto, note);
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

    [HttpPut("{id}/name")]
    public async Task<IActionResult> UpdateCompanyName(int id, [FromBody] UpdateCompanyNameDto dto)
    {
        var entity = await db.CompanyItems.FindAsync(id);
        if (entity == null) return NotFound();

        entity.CompanyName = dto.Name;
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("by-name/{companyName}")]
    public async Task<ActionResult<int>> GetCompanyIdByName(string companyName)
    {
        var entity = await db.CompanyItems
            .Where(x => x.CompanyName == companyName)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (entity == 0)
            return NotFound($"Spółka o nazwie '{companyName}' nie została znaleziona");

        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompanyItem(int id)
    {
        var item = await db.CompanyItems
            .Include(x => x.Summary)
            .Include(x => x.Notes)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        db.CompanySummaries.RemoveRange(item.Summary);
        db.CompanyNotes.RemoveRange(item.Notes);
        db.CompanyItems.Remove(item);

        await db.SaveChangesAsync();
        return Ok();
    }

    public class UpdateCompanyNameDto
    {
        public string Name { get; set; }
    }
}