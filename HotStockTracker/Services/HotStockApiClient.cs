using EFCore.Models;
using HotStockTracker.MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace HotStockTracker.Services;

public class HotStockApiClient(HttpClient http)
{
    public async Task<ObservableCollection<HotStockDayViewModel>> GetHotStockDaysAsync()
    {
        var response = await http.GetFromJsonAsync<List<HotStockDayViewModel>>("http://localhost:5153/api/HotStockDay");
        return new ObservableCollection<HotStockDayViewModel>(response ?? []);
    }

    public async Task<int> AddFullHotStockDayAsync(HotStockDay dayWithItems)
    {
        var result = await http.PostAsJsonAsync("http://localhost:5153/api/HotStockDay/day", dayWithItems);
        result.EnsureSuccessStatusCode();

        var created = await result.Content.ReadFromJsonAsync<HotStockDay>();
        return created?.Id ?? 0;
    }
}
