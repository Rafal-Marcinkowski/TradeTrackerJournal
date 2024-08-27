using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class DatesEqualToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3 || values[0] is not DateTime date1 || values[1] is not DateTime date2 || values[2] != null && values[2] is not string)
        {
            return Visibility.Collapsed;
        }

        if (values[2] is string desc && desc == "Transakcja z archiwum.")
        {
            return Visibility.Collapsed;
        }

        return date1.Date == date2.Date ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
