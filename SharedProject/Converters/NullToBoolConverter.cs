using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class NullToBoolConverter : IValueConverter
{
    public bool TrueWhenNull { get; set; } = true;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? TrueWhenNull : !TrueWhenNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
