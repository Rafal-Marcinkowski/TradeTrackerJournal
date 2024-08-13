using MahApps.Metro.IconPacks;
using System.Globalization;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class TransactionStatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isClosed = (bool)value;
        return isClosed ? PackIconModernKind.BookPerspective : PackIconModernKind.BookHardcoverOpenWriting;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
