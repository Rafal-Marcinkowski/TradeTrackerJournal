using SharedProject.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HotStockTracker.Services;

public class HotStockApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public HotStockApiClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("http://localhost:5153/");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public async Task<List<HotStockDayDto>> GetHotStockDaysAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/HotStockDay");

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"API Error: {response.StatusCode}");
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"API Response: {content}");

            var result = JsonSerializer.Deserialize<List<HotStockDayDto>>(content, _jsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in GetHotStockDaysAsync: {ex}");
            return [];
        }
    }

    public async Task<HotStockDayDto> AddHotStockDayAsync(HotStockDayDto dayDto)
    {
        var response = await _http.PostAsJsonAsync("api/HotStockDay", dayDto, _jsonOptions);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<HotStockDayDto>(_jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task UpdateDaySummaryAsync(HotStockDayDto dto)
    {
        var dateStr = dto.Date.ToString("yyyy-MM-dd");
        var response = await _http.PutAsJsonAsync($"api/HotStockDay/{dateStr}", dto, _jsonOptions);
        Debug.WriteLine($"PUT to: api/HotStockDay/{dateStr}");
        Debug.WriteLine($"Status: {response.StatusCode}");
        response.EnsureSuccessStatusCode();
    }
}
