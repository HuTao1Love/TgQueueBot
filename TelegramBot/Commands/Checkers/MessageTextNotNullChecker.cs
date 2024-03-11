using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class MessageTextNotNullChecker : CheckerBase
{
    protected override IEnumerable<CheckerBase>? SubCheckers => null;

    protected override Task<bool> Check(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        return Task.FromResult(update?.Message?.Text is not null && update?.Message?.From is not null);
    }
}