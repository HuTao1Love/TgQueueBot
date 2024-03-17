using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class ResetQueueCommand(BotConfiguration configuration, IQueueService service, IUserRepository userRepository) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CallbackDataChecker(KeyboardButton.ResetKeyboardButton.ResetCallback),
        new UserIsAdminChecker(userRepository, "You are not an admin"),
    };

    public override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.CallbackQuery?.Message;
        if (message is null) return;

        Queue? queue = await service.FindQueue(
            message.Chat.Id,
            message.MessageId);

        if (queue is null) return;

        if (queue.Users.All(i => i is null)) return;

        queue = await service.ResetQueue(queue);

        await update.TelegramBotClient.EditTextAsync(
            message.Chat.Id,
            message.MessageId,
            queue.ToString(),
            queue.Markup(configuration.MaxItemsPerKeyboardLine).ToTelegramKeyboardMarkup(),
            token);
    }
}