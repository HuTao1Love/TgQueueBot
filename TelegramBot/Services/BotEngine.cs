using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Rules;

namespace TelegramBot.Services;

public class BotEngine(ITelegramBotClient telegramBotClient, IEnumerable<ICommand> commands, BotContext botContext, IServiceProvider provider)
{
    private IReadOnlyCollection<ICommand> _commands = commands.ToList();
    public async Task ListenForMessagesAsync()
    {
        var receiverOptions = new ReceiverOptions(); // receive all update types

        User me = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"Start listening on @{me.Username}");

        IEnumerable<BotCommand> newMessages = commands
            .Select(c => c.GetType().GetCustomAttribute<NewMessageAttribute>())
            .Where(i => i is not null && i.Name is not null && i.Description is not null)
            .Select(i => new BotCommand { Command = i!.Name!, Description = i.Description! });

        await telegramBotClient.SetMyCommandsAsync(newMessages);

        IUpdateHandler updateHandler = new UpdateHandlerNotAwaitUpdatesProxy(
            new UpdateHandlerCatchExceptionsProxy(
                new UpdateHandler(provider, _commands)));

        await telegramBotClient.ReceiveAsync(
            updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: botContext.UpdateReceivingTokenSource.Token);

        Console.WriteLine($"Stopped listening on {me.Username}");
    }
}