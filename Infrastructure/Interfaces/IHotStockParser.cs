using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface IHotStockParser
{
    Task<List<HotStockItemDto>> ParseHotStocks(string html);
}