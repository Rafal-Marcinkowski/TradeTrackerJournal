
namespace SharedProject.Services.Filtering;

public interface IFilterService
{
    void ApplyFilter(string filterValue);
    Predicate<object> GetFilterPredicate();
    void Refresh();
}