using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore.Models;

public class NotepadCompanyItem
{
    public int Id { get; set; }

    [MaxLength(35)]
    public string CompanyName { get; set; } = string.Empty;

    [JsonIgnore]
    public CompanySummary? Summary { get; set; }

    [JsonIgnore]
    public ICollection<Note>? Notes { get; set; } = [];
}
