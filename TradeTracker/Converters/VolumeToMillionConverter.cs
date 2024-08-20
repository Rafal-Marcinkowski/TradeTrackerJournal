using System.Globalization;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class VolumeToMillionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int volume)
        {
            decimal volumeInMillions = (decimal)volume / 1_000_000;
            return $"{volumeInMillions:N3} mln";
        }

        if (value is decimal volume2)
        {
            decimal volumeInMillions = volume2 / 1_000_000;
            return $"{volumeInMillions:N3} mln";
        }
        return "0.000 mln";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
