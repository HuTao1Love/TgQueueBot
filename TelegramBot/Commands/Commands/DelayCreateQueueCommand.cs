using System.Globalization;
using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class DelayCreateQueueCommand(
    BotConfiguration configuration,
    BotContext context,
    IUserRepository userRepository,
    IQueueService queueService,
    CultureInfo cultureInfo) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "delaycreateq", "delaycreatequeue", "delaystartq", "delaystartqueue"),
        new UserIsAdminChecker(userRepository, "You are not admin"),
    };

    public override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.Message;
        if (message is null) return;

        var text = string.Concat(message.Text ?? string.Empty
                .Skip(configuration.BotPrefix.Length))
            .Split(' ', StringSplitOptions.TrimEntries)
            .ToList();

        if (text.Count < 3)
        {
            await update.AnswerText(
                "Usage: /delaycreatequeue <time, HH:MM:SS or HH:MM or int value - seconds> <size, default=25> <name>");
            return;
        }

        int queueSize = configuration.DefaultQueueSize;

        bool isUpdatedQueueSize = false;

        if (text.Count > 3 && int.TryParse(text[2], out queueSize))
        {
            isUpdatedQueueSize = true;
            queueSize = int.Min(queueSize, configuration.MaxQueueSize);
        }

        DateTime time = text[1].DateTimeFromString(cultureInfo);
        string name = text[isUpdatedQueueSize ? 3 : 2];

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

        Message sent = await update.AnswerWithKeyboard(
            $"Queue will be created at: {time.ToString(cultureInfo)}",
            new KeyboardMarkup(configuration.MaxItemsPerKeyboardLine)
                .AddItems(new KeyboardButton.CancelKeyboardButton())
                .ToTelegramKeyboardMarkup());

        var id = new MessageIdentifier(sent.Chat.Id, sent.MessageId);

        CancellationTokenSource cts = context.CancellationTokenDictionary.GetOrAdd(
                id,
                new CancellationTokenSource());
        await Task.Delay(time - DateTime.Now, cts.Token);
        await CreateQueueCommand.CreateQueue(
            update,
            queueService,
            sent,
            name,
            queueSize,
            configuration.MaxItemsPerKeyboardLine,
            cts.Token);
        context.CancellationTokenDictionary.TryRemove(new KeyValuePair<MessageIdentifier, CancellationTokenSource>(id, cts));
    }
}