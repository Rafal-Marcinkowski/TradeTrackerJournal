using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharedProject.Converters;

public class DatesEqualToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not DateTime date1 || values[1] is not DateTime date2)
        {
            return Visibility.Collapsed;
        }

        if (values.Length == 3 && values[2] is string desc && desc == "Transakcja z archiwum.")
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
