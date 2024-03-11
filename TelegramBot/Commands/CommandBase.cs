using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Commands;

public abstract class CommandBase
{
    protected abstract IEnumerable<CheckerBase> Checkers { get; }

    public async Task<bool> CheckAndExecute(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        bool canExecute = await Checkers
            .Select(c => c.CheckWithSubCheckers(botClient, update, token))
            .CheckAllAsync();

        if (!canExecute) return false;

        await Execute(new ClientUpdate(update, botClient), token);
        return true;
    }

    protected abstract Task Execute(ClientUpdate update, CancellationToken token);
}