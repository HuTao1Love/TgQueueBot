using System.Globalization;

namespace TelegramBot.Services;

public static class DateTimeHelper
{
    private static readonly string[] Formats = { "hh:mm", "hh:mm:ss" };

    public static DateTime DateTimeFromString(this string dateTimeObject, CultureInfo cultureInfo)
    {
        if (int.TryParse(dateTimeObject, out int fromIntValue))
        {
            if (fromIntValue <= 0) throw new ArgumentException("DateTime must be positive");
            return DateTime.Now.AddSeconds(fromIntValue);
        }

        if (!TimeOnly.TryParseExact(
                dateTimeObject,
                Formats,
                cultureInfo,
                DateTimeStyles.None,
                out TimeOnly fromTimeOnlyValue)) throw new ArgumentException("Invalid format of DateTime");

        var value = new DateTime(DateOnly.FromDateTime(DateTime.Now), fromTimeOnlyValue);
        if (value < DateTime.Now) value = value.AddDays(1);

        return value;
    }
}