using Contracts;
using Telegram.Bot;
using TelegramBot.Rules;
using TelegramBot.Services;

namespace TelegramBot.Commands;

[NewMessage("/shutdown", Name = "shutdown", Description = "Shutdown bot")]
[UserIsCreatorRule("Only for Hu Tao")]
public class ShutdownCommand(BotConfiguration configuration, BotContext context) : ICommand
{
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