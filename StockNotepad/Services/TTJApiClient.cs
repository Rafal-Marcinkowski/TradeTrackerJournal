using StockNotepad.MVVM.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockNotepad.Services;

public class TTJApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public TTJApiClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("http://localhost:5153/");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public async Task<List<NotepadCompanyItemDto>> GetNotepadCompanyItemsAsync()
    {
        var response = await _http.GetAsync("api/NotepadCompanyItem");
        if (!response.IsSuccessStatusCode) return [];

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<NotepadCompanyItemDto>>(stream, _jsonOptions) ?? [];
    }

    public async Task<bool> AddNotepadCompanyItemAsync(NotepadCompanyItemDto companyItemDto)
    {
        var response = await _http.PostAsJsonAsync("api/NotepadCompanyItem", companyItemDto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSummaryAsync(int id, CompanySummaryDto summaryDto)
    {
        var response = await _http.PutAsJsonAsync($"api/NotepadCompanyItem/{id}/summary", summaryDto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<int?> AddNoteAsync(int companyItemId, NoteDto noteDto)
    {
        var response = await _http.PostAsJsonAsync($"api/NotepadCompanyItem/{companyItemId}/notes", noteDto, _jsonOptions);
        if (!response.IsSuccessStatusCode)
            return null;

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<int>(stream, _jsonOptions);
    }

    public async Task<bool> UpdateNoteAsync(int noteId, NoteDto noteDto)
    {
        var response = await _http.PutAsJsonAsync($"/notes/{noteId}", noteDto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteNoteAsync(int noteId)
    {
        var response = await _http.DeleteAsync($"api/NotepadCompanyItem/note/{noteId}");
        return response.IsSuccessStatusCode;
    }
}

