using SharedProject.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HotStockTracker.Services;

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

    public async Task<bool> AddHotStockDayAsync(HotStockDayDto dayDto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/HotStockDay", dayDto, _jsonOptions);

            Debug.WriteLine($"POST to: api/HotStockDay");
            Debug.WriteLine($"Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error response: {errorContent}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in AddHotStockDayAsync: {ex}");
            return false;
        }
    }

    public async Task<bool> UpdateHotStockDayAsync(HotStockDayDto dto)
    {
        try
        {
            var dateStr = dto.Date.ToString("yyyy-MM-dd");
            Debug.WriteLine($"Updating HotStockDay for date: {dateStr}");
            Debug.WriteLine($"Items: {dto.HotStockItems?.Count}");
            var response = await _http.PutAsJsonAsync($"api/HotStockDay/{dateStr}", dto, _jsonOptions);

            Debug.WriteLine($"PUT to: api/HotStockDay/{dateStr}");
            Debug.WriteLine($"Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error response: {errorContent}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in UpdateDay: {ex}");
            return false;
        }
    }
}
