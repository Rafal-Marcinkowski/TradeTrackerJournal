using System.Globalization;

namespace SharedProject.Extensions;

public static class StringExtensions
{
    public static string CleanNumberInput(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var cleaned = new string([.. input.Where(c => char.IsDigit(c) || c == '.' || c == ',')]);

        if (cleaned != input)
            return string.Empty;

        return cleaned.Replace(",", ".");
    }

    public static bool TryParseCleanDecimal(this string input, out decimal value)
    {
        var cleaned = input.CleanNumberInput();

        if (string.IsNullOrEmpty(cleaned))
        {
            value = 0m;
            return false;
        }

        return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
    }
}