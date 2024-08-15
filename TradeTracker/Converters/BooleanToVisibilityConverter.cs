using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradeTracker.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Sprawdzenie czy parametr konwertera jest ustawiony
        bool isInverse = parameter != null && bool.Parse(parameter.ToString());

        if (value is bool boolValue)
        {
            // Jeśli isInverse to odwróć wartość, jeśli nie, używaj jej bez zmian
            return (boolValue && !isInverse) || (!boolValue && isInverse) ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isInverse = parameter != null && bool.Parse(parameter.ToString());

        if (value is Visibility visibility)
        {
            // Zwrócenie wartości odwrotnej w przypadku gdy isInverse jest true
            return (visibility == Visibility.Visible && !isInverse) || (visibility != Visibility.Visible && isInverse);
        }

        return false;
    }
}


//public class BooleanToVisibilityConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value is bool boolValue)
//        {
//            return boolValue ? Visibility.Visible : Visibility.Collapsed;
//        }

//        return Visibility.Collapsed;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value is Visibility visibility)
//        {
//            return visibility == Visibility.Visible;
//        }

//        return false;
//    }
//}
