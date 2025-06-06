using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class PolishDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var polishCulture = new CultureInfo("pl-PL");
            return date.ToString("dddd, dd MMMM yyyy", polishCulture);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
