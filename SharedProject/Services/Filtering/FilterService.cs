using System.Reflection;

namespace SharedProject.Services.Filtering;

public class FilterService<T>(Func<T, bool> customFilter = null) : IFilterService
{
    private string _currentFilterValue;
    private readonly Func<T, bool> _customFilter = customFilter;

    private readonly PropertyInfo[] _filterableProperties = [.. typeof(T)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<FilterableAttribute>() != null)];

    public void ApplyFilter(string filterValue)
    {
        _currentFilterValue = filterValue;
    }

    public Predicate<object> GetFilterPredicate()
    {
        if (string.IsNullOrWhiteSpace(_currentFilterValue))
            return _ => true;

        var filterParts = _currentFilterValue.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        return item =>
        {
            if (item is not T typedItem)
                return false;

            if (_customFilter != null && !_customFilter(typedItem))
                return false;

            return filterParts.All(part =>
                _filterableProperties.Any(prop =>
                    MatchesPropertyValue(prop, typedItem, part)));
        };
    }

    private static bool MatchesPropertyValue(PropertyInfo prop, T item, string searchPart)
    {
        var value = prop.GetValue(item);

        if (value == null)
            return false;

        if (prop.PropertyType == typeof(DateTime))
        {
            return ((DateTime)value).ToString("d").Contains(searchPart, StringComparison.OrdinalIgnoreCase);
        }

        return value.ToString()!.Contains(searchPart, StringComparison.OrdinalIgnoreCase);
    }

    public void Refresh()
    {
        // Optional: Clear any cached data if needed
    }
}
