using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBot.Services;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
public class UpdateHandlerNotAwaitUpdatesProxy(IUpdateHandler baseHandler) : IUpdateHandler
{
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        baseHandler.HandleUpdateAsync(botClient, update, cancellationToken);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        await baseHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
    }
}