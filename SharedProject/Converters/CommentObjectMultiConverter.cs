using SharedProject.Interfaces;
using SharedProject.Models;
using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class CommentObjectMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
            return null;

        if (values[0] is not Comment comment || values[1] is not ICommentable parent)
            return null;

        return new Tuple<Comment, ICommentable>(comment, parent);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

