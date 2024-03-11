using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Commands;

public abstract class CheckerBase
{
    protected abstract IEnumerable<CheckerBase>? SubCheckers { get; }

    public async Task<bool> CheckWithSubCheckers(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        bool subCheckersResult = SubCheckers is null || await SubCheckers
            .Select(i => i.CheckWithSubCheckers(telegramBotClient, update, token))
            .CheckAllAsync();

        return subCheckersResult && await Check(telegramBotClient, update, token);
    }

    protected abstract Task<bool> Check(ITelegramBotClient telegramBotClient, Update update, CancellationToken token);
}