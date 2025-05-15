using MahApps.Metro.IconPacks;
using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class IsTrackingToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isTracking = value is bool b && b;
        return isTracking
            ? PackIconGameIconsKind.VelociraptorTracks
            : PackIconGameIconsKind.CheckMark;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
