using Contracts;
using Telegram.Bot;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class ShutdownCommand(BotConfiguration configuration, BotContext context) : ICommand
{
    public string? Name => "shutdown";
    public string? Description => "Shutdown bot";

    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "shutdown"),
        new UserIsCreatorChecker(configuration, "Only for Hu Tao"),
    };

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        await update.AnswerText("Starting shutdown - stopping queues and receiving updates");

        await context.UpdateReceivingTokenSource.CancelAsync();

        ITelegramBotClient client = update.TelegramBotClient;

        foreach (KeyValuePair<MessageIdentifier, CancellationTokenSource> dTokenSource in context.CancellationTokenDictionary)
        {
            await dTokenSource.Key.SafeUnpinMessageAsync(client, token);
            await client.EditTextAsync(
                dTokenSource.Key.ChatId,
                dTokenSource.Key.MessageId,
                "Stopped because of bot shutdown",
                null);
            await dTokenSource.Value.CancelAsync();
        }
    }
}