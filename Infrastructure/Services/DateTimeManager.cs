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

    public static void ShouldCheckForData(DateTime now, out DateTime today, out DateTime previousWorkDay)
    {
        today = now.Date;
        previousWorkDay = DateTime.MinValue;

        if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
        {
            if (now.DayOfWeek == DayOfWeek.Sunday ||
                (now.DayOfWeek == DayOfWeek.Saturday && now.TimeOfDay < TimeSpan.FromHours(9)))
            {
                previousWorkDay = GetLastFriday(now);
            }
            return;
        }

        var tradingEndTime = today.AddHours(17).AddMinutes(5);
        var tradingStartTime = today.AddHours(9);

        if (now < tradingStartTime)
        {
            previousWorkDay = GetPreviousWorkDay(now);
        }
    }

    public static bool ShouldUpdateToday(DateTime now)
    {
        if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            return false;

        var today = now.Date;
        var tradingEndTime = today.AddHours(17).AddMinutes(5);

        return now >= tradingEndTime;
    }

    private static DateTime GetLastFriday(DateTime date)
    {
        var friday = date.AddDays(-(int)date.DayOfWeek - 2);
        if (friday.DayOfWeek != DayOfWeek.Friday)
            friday = friday.AddDays(-7);
        return friday.Date;
    }

    private static DateTime GetPreviousWorkDay(DateTime date)
    {
        var previousDay = date.AddDays(-1);
        while (previousDay.DayOfWeek == DayOfWeek.Saturday || previousDay.DayOfWeek == DayOfWeek.Sunday)
        {
            previousDay = previousDay.AddDays(-1);
        }
        return previousDay.Date;
    }

    public static bool ShouldCheckForData(DateTime now, out DateTime targetDate)
    {
        targetDate = DateTime.MinValue;

        if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
        {
            if (now.DayOfWeek == DayOfWeek.Sunday ||
                (now.DayOfWeek == DayOfWeek.Saturday && now.TimeOfDay < TimeSpan.FromHours(9)))
            {
                targetDate = GetLastFriday(now);
                return true;
            }
            return false;
        }

        var today = now.Date;
        var tradingEndTime = today.AddHours(17).AddMinutes(5);
        var tradingStartTime = today.AddHours(9);

        if (now >= tradingStartTime && now < tradingEndTime)
        {
            targetDate = today;
            return false;
        }

        if (now >= tradingEndTime)
        {
            targetDate = today;
            return true;
        }

        if (now < tradingStartTime)
        {
            targetDate = GetPreviousWorkDay(now);
            return true;
        }

        return false;
    }

    public static bool IsWithinTradingTimeWindow(DateTime now, out DateTime targetDate)
    {
        var today = now.Date;
        var startTime = today.AddHours(17).AddMinutes(6); // 17:06
        var endTime = today.AddHours(8).AddMinutes(59);   // 08:59

        // Ignoruj weekendy (sobota i niedziela)
        if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
        {
            targetDate = DateTime.MinValue;
            return false;
        }

        // Jeśli jesteśmy w przedziale wieczornym (po 17:06)
        if (now >= startTime)
        {
            // Jeśli to piątek wieczorem, target to piątek
            if (now.DayOfWeek == DayOfWeek.Friday)
            {
                targetDate = today;
                return true;
            }

            // W przeciwnym razie ustaw dzisiejszą datę
            targetDate = today;
            return true;
        }

        // Jeśli jesteśmy w przedziale porannym (przed 09:00)
        if (now <= endTime)
        {
            // Znajdź poprzedni dzień roboczy
            var previousDay = today.AddDays(-1);
            while (previousDay.DayOfWeek == DayOfWeek.Saturday ||
                   previousDay.DayOfWeek == DayOfWeek.Sunday)
            {
                previousDay = previousDay.AddDays(-1);
            }

            targetDate = previousDay;
            return true;
        }

        targetDate = DateTime.MinValue;
        return false;
    }
}
