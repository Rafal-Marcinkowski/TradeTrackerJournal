using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore.Models;

public class Note
{
    public int Id { get; set; }

    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int NotepadCompanyItemId { get; set; }

    [JsonIgnore]
    public NotepadCompanyItem? NotepadCompanyItem { get; set; }
}
