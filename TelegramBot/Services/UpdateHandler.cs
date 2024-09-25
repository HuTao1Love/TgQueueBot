using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Rules;

namespace TelegramBot.Services;

public class UpdateHandler(IEnumerable<ICommand> commands) : IUpdateHandler
{
    private IReadOnlyCollection<ICommand> _commands = commands.ToList();

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

        ICommand? command = (await Task.WhenAll(_commands
            .Select(c => new
            {
                Command = c,
                Rules = c.GetType().GetCustomAttributes<RuleAttribute>(true),
            })
            .Select(async c => new
            {
                Command = c.Command,
                CheckResult = await Task.WhenAll(c.Rules.Select(a => a.Check(clientUpdate, cancellationToken))),
            })))
            .FirstOrDefault(c => c.CheckResult.All(i => i))?
            .Command;

        if (command is not null)
        {
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