using System.Globalization;

namespace TelegramBot.Services;

public static class DateTimeHelper
{
    private static readonly string[] Formats = { "HH:MM", "HH:MM:SS" };

    public static DateTime DateTimeFromString(this string dateTimeObject)
    {
        if (int.TryParse(dateTimeObject, out int fromIntValue))
        {
            if (fromIntValue <= 0) throw new ArgumentException("DateTime must be positive");
            return DateTime.Now.AddSeconds(fromIntValue);
        }

        if (!TimeOnly.TryParseExact(
                dateTimeObject,
                Formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out TimeOnly fromTimeOnlyValue)) throw new ArgumentException("Invalid format of time");

        var value = new DateTime(DateOnly.FromDateTime(DateTime.Now), fromTimeOnlyValue);
        if (value < DateTime.Now) value = value.AddDays(1);

        return value;
    }
}