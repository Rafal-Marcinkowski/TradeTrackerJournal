using SharedProject.Models;
using System.Windows.Data;

namespace SharedProject.Converters;

public class CommentObjectMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is Comment comment)
        {
            if (values[1] is Transaction transaction)
            {
                return Tuple.Create(comment, transaction);
            }

            else if (values[1] is Event eventObj)
            {
                return Tuple.Create(comment, eventObj);
            }
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
