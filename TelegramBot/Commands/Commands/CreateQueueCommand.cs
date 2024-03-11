using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using Models;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class CreateQueueCommand(BotConfiguration configuration, IUserRepository userRepository, IQueueService queueService) : CommandBase
{
    protected override IEnumerable<CheckerBase> Checkers { get; } = new CheckerBase[]
    {
        new CommandChecker(configuration.BotPrefix, "createq", "createqueue", "startq", "startqueue"),
        new UserIsAdminChecker(userRepository),
    };

    protected override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        var text = update!.Message!.Text!
            .Split(' ', StringSplitOptions.TrimEntries)
            .ToList();

        if (text.Count < 2)
        {
            await update.AnswerText($"Usage: /createqueue <size, default={configuration.DefaultQueueSize}> <name>");
            return;
        }

        int queueSize = configuration.DefaultQueueSize;

        bool isUpdatedQueueSize = false;

        if (text.Count > 2 && int.TryParse(text[1], out queueSize))
        {
            isUpdatedQueueSize = true;
            queueSize = int.Min(queueSize, configuration.MaxQueueSize);
        }

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

        Queue queue = await queueService.CreateQueue(update!.Message!.Chat!.Id, update!.Message!.MessageId, name, queueSize);
        await update.AnswerWithKeyboard(
            queue.ToString(),
            queue.Markup(configuration.MaxItemsPerKeyboardLine).ToTelegramKeyboardMarkup());
    }
}