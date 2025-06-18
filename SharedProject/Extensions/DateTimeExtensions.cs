namespace SharedProject.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimToSeconds(this DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, dateTime.Minute, dateTime.Second,
            dateTime.Kind);
    }
}
