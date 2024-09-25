using Contracts;
using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Rules;
using TelegramBot.Services;

namespace TelegramBot.Commands;

[NewMessage("/createq", "/createqueue", "/startq", "/startqueue", Name = "createq", Description = "Create new queue")]
[UserIsAdminRule("You are not admin")]
public class CreateQueueCommand(BotConfiguration configuration, IQueueService queueService) : ICommand
{
    public static async Task CreateQueue(ClientUpdate update, IQueueService service, Message sent, string name, int size, int maxItemsPerKeyboardLine, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(sent);

        Queue queue = await service.CreateQueue(sent.Chat.Id, sent.MessageId, name, size);
        await sent.SafePinMessageAsync(update.TelegramBotClient, token: token);
        await update.TelegramBotClient.EditTextAsync(
            sent.Chat.Id,
            sent.MessageId,
            text: queue.ToString(),
            markup: queue.Markup(maxItemsPerKeyboardLine).ToTelegramKeyboardMarkup(),
            token: token);
    }

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.Message;
        if (message is null) return;

        var text = string.Concat(message.Text ?? string.Empty)
            .Split(' ', StringSplitOptions.TrimEntries)
            .ToList();

        if (text.Count < 2)
        {
            await update.AnswerText($"Usage: /createqueue <size, default=25> <name>");
            return;
        }

        int queueSize = configuration.DefaultQueueSize;

        bool isUpdatedQueueSize = false;

        if (text.Count > 2 && int.TryParse(text[1], out queueSize))
        {
            isUpdatedQueueSize = true;
            queueSize = int.Min(queueSize, configuration.MaxQueueSize);
        }

        string name = text[isUpdatedQueueSize ? 2 : 1];

        if (name.Contains('/', StringComparison.InvariantCultureIgnoreCase))
        {
            await update.AnswerText($"Do not use / in queue name");
            return;
        }

        if (name.Length > configuration.MaxQueueNameLength)
        {
            await update.AnswerText($"Too long name");
            return;
        }

        Message sent = await update.AnswerText($"Queue will be created ...");

        await CreateQueue(update, queueService, sent, name, queueSize, configuration.MaxItemsPerKeyboardLine, token);
    }
}