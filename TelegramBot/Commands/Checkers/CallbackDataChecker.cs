using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class CallbackDataChecker(params string[] dataStartWith) : IChecker
{
    public Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        string? callback = update?.CallbackQuery?.Data;
        return Task.FromResult(callback is not null && dataStartWith.Any(
            i => callback.StartsWith(i, StringComparison.OrdinalIgnoreCase)));
    }
}