﻿using System.Windows.Data;
using TradeTracker.MVVM.Models;

namespace TradeTracker.Converters;

public class CommentTransactionMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is TransactionComment comment && values[1] is Transaction transaction)
        {
            return Tuple.Create(comment, transaction);
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
