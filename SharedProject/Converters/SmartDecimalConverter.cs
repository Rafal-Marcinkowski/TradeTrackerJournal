using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class SmartDecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "";

        if (value is decimal d)
            return d.ToString("0.###", culture);

        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}