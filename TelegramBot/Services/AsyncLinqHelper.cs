namespace TelegramBot.Services;

public static class AsyncLinqHelper
{
    public static async Task<bool> CheckAllAsync(this IEnumerable<Task<bool>> values)
    {
        return (await Task.WhenAll(values)).All(i => i);
    }

    // just for prettier - extension method
    public static async Task<IEnumerable<T>> TaskWhenAll<T>(this IEnumerable<Task<T>> values)
    {
        return await Task.WhenAll(values);
    }
}