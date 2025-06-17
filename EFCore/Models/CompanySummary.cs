using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore.Models;

public class CompanySummary
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    public int NotepadCompanyItemId { get; set; }

    [JsonIgnore]
    public NotepadCompanyItem? NotepadCompanyItem { get; set; }
}
