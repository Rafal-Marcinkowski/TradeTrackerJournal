using System.Globalization;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class BooleanToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue; // true enables the control, false disables it
        }
        return false; // Default to disabled if the value is not a boolean
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue; // Inverse conversion, same logic applies
        }
        return false; // Default to false if the value is not a boolean
    }
}
