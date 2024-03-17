using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Commands;

namespace TelegramBot;

public class BotEngine(ITelegramBotClient telegramBotClient, IEnumerable<CommandBase> commands)
{
    public async Task ListenForMessagesAsync()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>(), // receive all update types
        };

        telegramBotClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

        User me = await telegramBotClient.GetMeAsync();

        Console.WriteLine($"Start listening on @{me.Username}");
        Console.ReadLine();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (update.Message?.Text is { } messageText)
        {
            Console.WriteLine($"{messageText} at {update.Message.Chat.Id}, {update.Message.MessageId}");
        }

        if (update.CallbackQuery is { Message: not null } callbackQuery)
        {
            Console.WriteLine($"{callbackQuery.Data} at {callbackQuery.Message.Chat.Id}, {callbackQuery.Message.MessageId}");
        }

        var clientUpdate = new ClientUpdate(update, botClient);

        foreach (CommandBase command in commands)
        {
            if (!await command.Check(clientUpdate, cancellationToken)) continue;
            await command.Execute(clientUpdate, cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString(),
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}