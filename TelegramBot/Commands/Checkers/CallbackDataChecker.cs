using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class CallbackDataChecker(params string[] dataStartWith) : CheckerBase
{
    protected override IEnumerable<CheckerBase>? SubCheckers => null;

    protected override Task<bool> Check(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        string? callback = update?.CallbackQuery?.Data;
        return Task.FromResult(callback is not null && dataStartWith.Any(i => callback.StartsWith(i, StringComparison.InvariantCultureIgnoreCase)));
    }
}