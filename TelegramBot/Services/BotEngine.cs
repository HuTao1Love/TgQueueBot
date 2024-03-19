using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Commands;

namespace TelegramBot.Services;

#pragma warning disable SK1200
public class BotEngine(ITelegramBotClient telegramBotClient, IEnumerable<ICommand> commands, BotContext botContext)
{
    public async Task ListenForMessagesAsync()
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = { }, }; // receive all update types

        User me = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"Start listening on @{me.Username}");

        await telegramBotClient.SetMyCommandsAsync(commands
            .Where(i => i.Name is not null && i.Description is not null)
            .Select(i => new BotCommand
            {
                Command = i.Name!,
                Description = i.Description!,
            }));

        IUpdateHandler updateHandler = new UpdateHandlerNotAwaitUpdatesProxy(
            new UpdateHandlerCatchExceptionsProxy(
                new UpdateHandler(commands)));

        await telegramBotClient.ReceiveAsync(
            updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: botContext.UpdateReceivingTokenSource.Token);

        Console.WriteLine($"Stopped listening on {me.Username}");
    }
}