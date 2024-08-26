using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class VisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.All(v => v != null && !string.IsNullOrWhiteSpace(v.ToString()))
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
