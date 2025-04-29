using System.Globalization;

namespace Infrastructure.Services;

public static class DateTimeManager
{
    public async static Task<int> SetDuration(DateTime EntryDate)
    {
        TimeSpan timeSpan = DateTime.Now.Date - EntryDate.Date;
        return (int)timeSpan.TotalDays;
    }

    public static DateTime ParseEntryDate(string input)
    {
        var normalizedInput = input.Replace(" ", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Trim();

        if (normalizedInput.Length == 12)
        {
            string datePart = normalizedInput[..8];
            string timePart = normalizedInput.Substring(8, 4);

            string formattedDateTime = $"{datePart[..2]}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)} {timePart[..2]}:{timePart.Substring(2, 2)}";

            if (DateTime.TryParseExact(formattedDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
            {
                return fullDate;
            }
        }

        else if (normalizedInput.Length == 8)
        {
            string datePart = normalizedInput[..8];

            string formattedDate = $"{datePart[..2]}/{datePart.Substring(2, 2)}/{datePart.Substring(4, 4)}";

            if (DateTime.TryParseExact(formattedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            {
                return dateOnly;
            }
        }

        else if (normalizedInput.Length == 4 || normalizedInput.Length == 3)
        {
            string timePart = normalizedInput.Length == 4 ? normalizedInput : $"0{normalizedInput[..1]}{normalizedInput.Substring(1, 2)}";

            if (TimeSpan.TryParseExact(timePart, "hhmm", CultureInfo.InvariantCulture, out var timeOnly))
            {
                return DateTime.Now.Date.Add(timeOnly);
            }
        }

        return new DateTime(1900, 1, 1, 0, 0, 0);
    }
}
