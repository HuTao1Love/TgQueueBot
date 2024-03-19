using System.Globalization;

namespace TelegramBot.Services;

public static class DateTimeHelper
{
    private static readonly string[] Formats = { "HH:mm", "HH:mm:ss" };

    public static DateTime DateTimeFromString(this string dateTimeObject, CultureInfo? cultureInfo = null)
    {
        if (int.TryParse(dateTimeObject, out int fromIntValue))
        {
            if (fromIntValue <= 0) throw new FormatException("DateTime must be positive");
            return DateTime.Now.AddSeconds(fromIntValue);
        }

        Console.WriteLine(dateTimeObject);

        var fromTimeOnlyValue = TimeOnly.ParseExact(dateTimeObject, Formats, cultureInfo);

        var value = new DateTime(DateOnly.FromDateTime(DateTime.Now), fromTimeOnlyValue);
        if (value < DateTime.Now) value = value.AddDays(1);

        return value;
    }
}