using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Commands;

namespace TelegramBot.Services;

public class UpdateHandler(IEnumerable<ICommand> commands) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (update.Message?.Text is { } messageText)
        {
            Console.WriteLine($"{messageText} at {update.Message.Chat.Title}, {update.Message.From?.Username}, {update.Message.MessageId}");
        }

        if (update.CallbackQuery is { Message: not null } callbackQuery)
        {
            Console.WriteLine($"{callbackQuery.Data} at {callbackQuery.Message.Chat.Title}, {callbackQuery.Message.MessageId}, {callbackQuery.Data}");
        }

        var clientUpdate = new ClientUpdate(update, botClient);

        foreach (ICommand command in commands)
        {
            if (!await command.Check(clientUpdate, cancellationToken)) continue;
            await command.Execute(clientUpdate, cancellationToken);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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