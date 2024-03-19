using TelegramBot.Commands;

namespace TelegramBot.Services;

public static class CommandExtensions
{
    public static async Task<bool> Check(this ICommand command, ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(update);

        foreach (IChecker checker in command.Checkers)
        {
            if (!await checker.Check(update, token)) return false;
        }

        return true;
    }
}