using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Rules;

namespace TelegramBot.Services;

public class UpdateHandler(IServiceProvider provider, IEnumerable<ICommand> commands) : IUpdateHandler
{
    private readonly IReadOnlyCollection<ICommand> _commands = commands.ToList();

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
                    MainRule = c.GetType().GetCustomAttribute<MainRuleAttribute>() ??
                               throw new ArgumentException("Provide main rule"),
                    Rules = c.GetType().GetCustomAttributes<RuleAttribute>(true),
                })
                .Select(async c =>
                {
                    c.MainRule.Initialize(provider);

                    if (!await c.MainRule.Check(clientUpdate, cancellationToken))
                    {
                        return null;
                    }

                    foreach (RuleAttribute? rule in c.Rules)
                    {
                        rule.Initialize(provider);
                    }

                    bool[] result = await Task.WhenAll(c.Rules.Select(a => a.Check(clientUpdate, cancellationToken)));

                    return !result.All(i => i) ? null : c.Command;
                })))
            .FirstOrDefault(i => i is not null);

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