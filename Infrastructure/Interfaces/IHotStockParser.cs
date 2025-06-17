using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface IHotStockParser
{
    List<HotStockItemDto> Parse(string html, string market, DateTime date = default);
}