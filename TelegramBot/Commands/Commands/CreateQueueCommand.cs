using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class CreateQueueCommand(BotConfiguration configuration, IUserRepository userRepository, IQueueService queueService) : ICommand
{
    public string? Name => "createq";
    public string? Description => "Create new queue";

    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "createq", "createqueue", "startq", "startqueue"),
        new UserIsAdminChecker(userRepository, "You are not admin"),
    };

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

        var text = string.Concat(message.Text ?? string.Empty
                .Skip(configuration.BotPrefix.Length))
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

        if (name.Contains(configuration.BotPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            await update.AnswerText($"Do not use {configuration.BotPrefix} in queue name");
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