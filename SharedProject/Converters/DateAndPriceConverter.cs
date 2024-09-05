using System.Globalization;
using System.Windows.Data;

namespace SharedProject.Converters;

public class DateAndPriceConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is DateTime closeDate && values[1] is decimal avgSellPrice)
        {
            string formattedDate = closeDate.ToString("dd/MM/yyyy HH:mm");
            string formattedPrice = avgSellPrice != null ? $"Cena sprzedaży: {avgSellPrice.ToString().Replace(',', '.')}" : string.Empty;

            return $"Data zamknięcia: {formattedDate}\n{formattedPrice}";
        }

        return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
